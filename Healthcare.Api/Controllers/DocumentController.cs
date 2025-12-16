using Healthcare.Api.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IOcrService _ocrService;
        private readonly IAiService _aiService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IOcrService ocrService, IAiService aiService, ILogger<DocumentController> logger, IAuditService auditService)
        {
            _ocrService = ocrService;
            _aiService = aiService;
            _logger = logger;
            _auditService = auditService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            _logger.LogInformation("Receiving file: {FileName}", file.FileName);

            // 1. OCR Processing
            string extractedText = "OCR Failed or Skipped";
            try
            {
                using var stream = file.OpenReadStream();
                extractedText = await _ocrService.ExtractTextFromImageAsync(stream);
            }
            catch (Exception ex)
            {
               _logger.LogWarning(ex, "OCR Extraction failed. Proceeding with raw file.");
               extractedText = $"Error extracting text: {ex.Message}";
            }

            // 2. Map to FHIR DocumentReference
            var docRef = new DocumentReference
            {
                Status = DocumentReferenceStatus.Current,
                Date = DateTimeOffset.UtcNow, // Fixed: Use DateTimeOffset, not string
                Description = file.FileName,
                Content = new List<DocumentReference.ContentComponent>
                {
                    new DocumentReference.ContentComponent
                    {
                        Attachment = new Attachment
                        {
                            ContentType = file.ContentType,
                            Title = file.FileName
                        }
                    }
                },
                Context = new DocumentReference.ContextComponent
                {
                   // Removing related for now to avoid complexity if not needed
                }
            };

            // 3. AI Analysis
            var aiResult = await _aiService.AnalyzeDocumentAsync(extractedText);

            // 4. Trigger Training (Fire and Forget)
            _ = _aiService.TriggerTrainingAsync(extractedText, "LabReport");

            await _auditService.LogAsync("Document Upload", $"Uploaded file: {file.FileName} ({file.ContentType})");

            return Ok(new 
            { 
                Message = "Document Processed successfully", 
                OcrText = extractedText, 
                AiAnalysis = aiResult,
                FhirResource = docRef
            });
        }
    }
}
