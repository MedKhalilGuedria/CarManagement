using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarManagement.Models
{
    public class AutoPart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("Manufacturer")]
        public string? Manufacturer { get; set; }

        [BsonElement("Price")]
        public decimal? Price { get; set; }

        [BsonElement("Quantity")]
        public int? Quantity { get; set; }
    }
}
