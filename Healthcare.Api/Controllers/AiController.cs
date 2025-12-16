using Healthcare.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Healthcare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IMongoCollection<TrainingLog> _trainingLogs;
        private readonly Services.IAiService _aiService;
        private readonly Services.IAuditService _auditService;
        private readonly ILogger<AiController> _logger;

        public AiController(IMongoDatabase mongoDatabase, Services.IAiService aiService, ILogger<AiController> logger, Services.IAuditService auditService)
        {
            _trainingLogs = mongoDatabase.GetCollection<TrainingLog>("TrainingLogs");
            _aiService = aiService;
            _logger = logger;
            _auditService = auditService;
        }

        [HttpPost("training-log")]
        public async Task<IActionResult> LogTrainingResult([FromBody] TrainingLog log)
        {
            log.CreatedAt = DateTime.UtcNow;
            await _trainingLogs.InsertOneAsync(log);
            return Ok(new { Message = "Training log saved successfully", Id = log.Id });
        }

        [HttpGet("training-history")]
        public async Task<ActionResult<List<TrainingLog>>> GetTrainingHistory()
        {
            var logs = await _trainingLogs.Find(_ => true).SortByDescending(x => x.CreatedAt).ToListAsync();
            return Ok(logs);
        }

        [HttpPost("train-all")]
        public async Task<IActionResult> TriggerBulkTraining()
        {
            var result = await _aiService.TrainOnAllDataAsync();
            if (result.StartsWith("Error") || result.StartsWith("Cloud Error"))
            {
                return StatusCode(500, new { Message = result });
            }
            return Ok(new { Message = result });
        }
        [HttpPost("predict-risk")]
        public async Task<IActionResult> PredictRisk([FromBody] Core.Entities.Patient patient)
        {
            try 
            {
                var result = await _aiService.PredictPatientRiskAsync(patient);
                await _auditService.LogAsync("Risk Analysis", $"Risk analysis performed for patient {patient.FirstName} {patient.LastName} (Age: {patient.DateOfBirth})");
                return Ok(result); // Return as JSON string or object
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error predicting patient risk");
                return StatusCode(500, new { Message = "Prediction failed" });
            }
        }
    }
}
