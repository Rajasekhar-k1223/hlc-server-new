
using Microsoft.AspNetCore.Mvc;
using Healthcare.Api.Services;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/medical-records")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IAiService _aiService;
        private readonly ILogger<MedicalRecordsController> _logger;
        private readonly IWebHostEnvironment _environment;

        public MedicalRecordsController(IAiService aiService, ILogger<MedicalRecordsController> logger, IWebHostEnvironment environment)
        {
            _aiService = aiService;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadMedicalRecord(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // Sanitize filename to prevent directory traversal
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("File uploaded to {Path}. Starting AI analysis...", filePath);

            // Call AI Service
            var result = await _aiService.AnalyzeFileAsync(filePath);

            return Ok(new { 
                fileName = fileName, 
                filePath = filePath, 
                analysis = result 
            });
        }
    }
}
