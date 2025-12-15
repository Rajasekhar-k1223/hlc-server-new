using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Healthcare.Core.Entities
{
    public class TrainingLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string OcrText { get; set; } = string.Empty;
        public int Epochs { get; set; }
        public double FinalLoss { get; set; }
        public double FinalAccuracy { get; set; }
        public string ModelPath { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
