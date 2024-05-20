using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarManagement.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; } // Admin, User, Employee
    }
}
