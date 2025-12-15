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
        private readonly ILogger<AiController> _logger;

        public AiController(IMongoDatabase mongoDatabase, ILogger<AiController> logger)
        {
            _trainingLogs = mongoDatabase.GetCollection<TrainingLog>("TrainingLogs");
            _logger = logger;
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
    }
}
