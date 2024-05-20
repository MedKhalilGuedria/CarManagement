using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarManagement.Models
{
    public class Car
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Make")]
        public string? Make { get; set; }

        [BsonElement("Model")]
        public string? Model { get; set; }

        [BsonElement("Year")]
        public int? Year { get; set; }

        [BsonElement("VIN")]
        public string? VIN { get; set; } // Vehicle Identification Number
    }
}
