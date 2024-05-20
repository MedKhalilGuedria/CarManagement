using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CarManagement.Models
{
    public class AvailableSlot
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Date")]
        public DateTime? Date { get; set; }

        [BsonElement("Time")]
        public string? Time { get; set; } // e.g., "10:00 AM"

        [BsonElement("IsBooked")]
        public bool? IsBooked { get; set; } = false;
    }
}
