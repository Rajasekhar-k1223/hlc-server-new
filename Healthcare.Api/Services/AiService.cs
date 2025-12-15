using System.Net.Http.Json;
using Healthcare.Core.Entities;
using MongoDB.Driver;

namespace Healthcare.Api.Services
{
    public class AiService : IAiService
    {
        private readonly ILogger<AiService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IMongoCollection<TrainingLog> _trainingLogs;
        private readonly bool _useCloud;
        private readonly string _cloudUrl;

        public AiService(ILogger<AiService> logger, HttpClient httpClient, IConfiguration configuration, IMongoDatabase mongoDatabase)
        {
            _logger = logger;
            _httpClient = httpClient;
            _trainingLogs = mongoDatabase.GetCollection<TrainingLog>("TrainingLogs");
            
            var aiSettings = configuration.GetSection("AiSettings");
            _useCloud = aiSettings.GetValue<bool>("UseCloud");
            _cloudUrl = aiSettings.GetValue<string>("CloudBridgeUrl");
        }

        public async Task<string> AnalyzeDocumentAsync(string documentText)
        {
            if (_useCloud)
            {
                 // Call Cloud API
                 try 
                 {
                    var response = await _httpClient.PostAsJsonAsync($"{_cloudUrl}/predict", new { text = documentText });
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<dynamic>();
                        return $"Cloud AI: {result.GetProperty("result")}";
                    }
                    return $"Cloud Error: {response.StatusCode}";
                 }
                 catch (Exception ex)
                 {
                     _logger.LogError(ex, "Failed to connect to Cloud AI Bridge");
                     return "Error: Could not connect to Cloud AI.";
                 }
            }

            _logger.LogInformation("Analyzing document with length: {Length}", documentText.Length);
            // Simulate AI analysis
            return Task.FromResult($"AI Analysis Result: Document determined to be relevant. Tokens found: {documentText.Split(' ').Length}");
        }

        public async Task TriggerTrainingAsync(string documentText, string documentType)
        {
             if (_useCloud)
            {
                 try 
                 {
                    _logger.LogInformation("Calling Cloud AI for training...");
                    var response = await _httpClient.PostAsJsonAsync($"{_cloudUrl}/train", new { data = documentText, type = documentType });
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                        
                        // Capture metrics from Cloud
                        double acc = 0;
                        double loss = 0;
                        int ep = 0;

                        if (result.TryGetProperty("final_accuracy", out var accProp)) acc = accProp.GetDouble();
                        if (result.TryGetProperty("final_loss", out var lossProp)) loss = lossProp.GetDouble();
                        if (result.TryGetProperty("epochs", out var epProp)) ep = epProp.GetInt32();

                        // Save to DB (Replacing the need for Python->API validation)
                        var log = new TrainingLog
                        {
                            FileName = "Cloud Training Request",
                            OcrText = documentText.Length > 100 ? documentText[..100] : documentText,
                            Epochs = ep,
                            FinalAccuracy = acc,
                            FinalLoss = loss,
                            ModelPath = "Cloud_Colab_Session",
                            CreatedAt = DateTime.UtcNow
                        };

                        await _trainingLogs.InsertOneAsync(log);
                        _logger.LogInformation("Cloud Training Completed and Logged to DB. Accuracy: {Acc}", acc);
                    }
                 }
                 catch (Exception ex)
                 {
                     _logger.LogError(ex, "Failed to connect to Cloud AI Bridge");
                 }
                 return;
            }

            _logger.LogInformation("Triggering AI training for DocumentType: {Type}", documentType);
            return;
        }

        public async Task<string> AnalyzeFileAsync(string filePath)
        {
            try
            {
                if (_useCloud)
                {
                     // In a real scenario, we would upload the file.
                     // For this bridge demo, we'll send the file path as a reference (assuming same machine access) 
                     // or we can read the text if it's OCR'd.
                     // Let's assume we want to send the extracted text logic or just a signal.
                     // The requirement is "Online Only", so we send logic there.
                     return await AnalyzeDocumentAsync($"File: {Path.GetFileName(filePath)}");
                }

                _logger.LogInformation("Starting AI Analysis for file: {FilePath}", filePath);
                // Determines path to python script
                var scriptPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Healthcare.Api", "AiScripts", "predict.py"));

                if (!File.Exists(scriptPath))
                {
                    _logger.LogError("AI Script not found at: {Path}", scriptPath);
                    return "Error: AI Script not found.";
                }

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{scriptPath}\" \"{filePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = System.Diagnostics.Process.Start(startInfo);
                if (process == null) return "Error: Could not start python process.";

                using var reader = process.StandardOutput;
                using var errorReader = process.StandardError;
                
                var result = await reader.ReadToEndAsync();
                var errors = await errorReader.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (!string.IsNullOrEmpty(errors))
                {
                    _logger.LogWarning("Python AI Script Stderr: {Errors}", errors);
                }

                _logger.LogInformation("AI Analysis Output: {Output}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during AI analysis");
                return $"Error: {ex.Message}";
            }
        }
    }
}
