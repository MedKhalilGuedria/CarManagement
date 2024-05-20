using CarManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarManagement.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Payment> _payments;
        private readonly IMongoCollection<AutoPart> _autoParts;

        public OrderController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Cars");
            _orders = database.GetCollection<Order>("Orders");
            _payments = database.GetCollection<Payment>("Payments");
            _autoParts = database.GetCollection<AutoPart>("AutoParts");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var userId = User.FindFirstValue(ClaimTypes.Actor);

            // Calculate total price
            decimal totalPrice = 0;
            foreach (var item in order.AutoParts)
            {
                var autoPart = await _autoParts.Find(ap => ap.Id == item.AutoPartId).FirstOrDefaultAsync();
                if (autoPart == null)
                {
                    return NotFound(new { message = $"Auto part with ID {item.AutoPartId} not found" });
                }
                totalPrice += autoPart.Price * item.Quantity;
            }

            order.TotalPrice = totalPrice;
            order.OrderDate = DateTime.UtcNow;
            order.UserId = userId;

            await _orders.InsertOneAsync(order);
            return Ok(new { message = "Order created successfully", orderId = order.Id });
        }

        [HttpPost("pay/{orderId}")]
        public async Task<IActionResult> MakePayment(string orderId)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            // Assuming payment is successful, create payment record
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = order.TotalPrice,
                Date = DateTime.UtcNow
            };

            await _payments.InsertOneAsync(payment);

            // Update order with payment ID
            var update = Builders<Order>.Update.Set(o => o.PaymentId, payment.Id);
            await _orders.UpdateOneAsync(o => o.Id == orderId, update);

            return Ok(new { message = "Payment successful" });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserOrders(string userId)
        {
            var orders = await _orders.Find(o => o.UserId == userId).ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{orderId}/details")]
        public async Task<IActionResult> GetOrderDetails(string orderId)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            var orderDetails = new
            {
                Order = order,
                Payment = await _payments.Find(p => p.OrderId == orderId).FirstOrDefaultAsync()
            };

            return Ok(orderDetails);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orders.Find(_ => true).ToListAsync();
            return Ok(orders);
        }


    }
}
