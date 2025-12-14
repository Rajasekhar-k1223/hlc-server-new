namespace Healthcare.Api.Services
{
    public class AiService : IAiService
    {
        private readonly ILogger<AiService> _logger;

        public AiService(ILogger<AiService> logger)
        {
            _logger = logger;
        }

        public Task<string> AnalyzeDocumentAsync(string documentText)
        {
            _logger.LogInformation("Analyzing document with length: {Length}", documentText.Length);
            // Simulate AI analysis
            return Task.FromResult($"AI Analysis Result: Document determined to be relevant. Tokens found: {documentText.Split(' ').Length}");
        }

        public Task TriggerTrainingAsync(string documentText, string documentType)
        {
            _logger.LogInformation("Triggering AI training for DocumentType: {Type}", documentType);
            // In a real scenario, push this to a Queue (RabbitMQ) or call Python API
            return Task.CompletedTask;
        }

        public async Task<string> AnalyzeFileAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Starting AI Analysis for file: {FilePath}", filePath);

                // Determine path to python script
                // Assumes running from project root (hlc-server-new)
                // New Path: hlc-server-new/Healthcare.Api/AiScripts/predict.py
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
