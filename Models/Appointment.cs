using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarManagement.Models
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("UserId")]
        public string? UserId { get; set; }

        [BsonElement("SlotId")]
        public string? SlotId { get; set; }

        [BsonElement("Type")]
        public string? Type { get; set; }

        [BsonElement("Paid")]
        public bool? Paid { get; set; } = false;

        [BsonElement("PaymentId")]
        public string? PaymentId { get; set; }

        [BsonElement("CarId")]
        public string? CarId { get; set; } // Reference to the Car

        [BsonElement("Status")]
        public string Status { get; set; } = "in progress";

        [BsonElement("Details")]
        public string? Details { get; set; }  // Detailed description of the car's state
    }
}
