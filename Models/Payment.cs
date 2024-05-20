using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarManagement.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("UserId")]
        public string? UserId { get; set; }

        [BsonElement("AppointmentId")]
        public string? AppointmentId { get; set; }

        [BsonElement("Amount")]
        public decimal Amount { get; set; }

        [BsonElement("OrderId")]
        public string? OrderId { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; }
    }
}
