using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CarManagement.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("UserId")]
        public string? UserId { get; set; }

        [BsonElement("AutoParts")]
        public List<OrderItem> AutoParts { get; set; }

        [BsonElement("TotalPrice")]
        public decimal? TotalPrice { get; set; }

        [BsonElement("OrderDate")]
        public DateTime? OrderDate { get; set; }

        [BsonElement("PaymentId")]
        public string? PaymentId { get; set; }
    }

    public class OrderItem
    {
        [BsonElement("AutoPartId")]
        public string AutoPartId { get; set; }

        [BsonElement("Quantity")]
        public int Quantity { get; set; }
    }
}
