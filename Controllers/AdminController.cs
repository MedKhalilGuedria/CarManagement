using CarManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CarManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMongoCollection<AvailableSlot> _slots;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IMongoClient mongoClient, ILogger<AdminController> logger)
        {
            var database = mongoClient.GetDatabase("Cars");
            _slots = database.GetCollection<AvailableSlot>("AvailableSlots");
            _logger = logger;
        }

        [HttpPost("addSlot")]
        public async Task<IActionResult> AddSlot([FromBody] AvailableSlot slot)
        {
            await _slots.InsertOneAsync(slot);
            _logger.LogInformation($"Slot added: {slot.Date} {slot.Time}");
            return Ok(new { message = "Slot added successfully" });
        }

        [HttpGet("getSlots")]
        public async Task<IActionResult> GetSlots()
        {
            var slots = await _slots.Find(s => !s.IsBooked).ToListAsync();
            return Ok(slots);
        }

        [HttpDelete("deleteSlot/{id}")]
        public async Task<IActionResult> DeleteSlot(string id)
        {
            var result = await _slots.DeleteOneAsync(s => s.Id == id);
            if (result.DeletedCount > 0)
            {
                return Ok(new { message = "Slot deleted successfully" });
            }
            return NotFound(new { message = "Slot not found" });
        }

        [HttpPut("updateSlot/{id}")]
        public async Task<IActionResult> UpdateSlot(string id, [FromBody] AvailableSlot updatedSlot)
        {
            var existingSlot = await _slots.Find(s => s.Id == id).FirstOrDefaultAsync();
            if (existingSlot == null)
            {
                return NotFound(new { message = "Slot not found" });
            }

            // Update the slot properties
            existingSlot.Date = updatedSlot.Date;
            existingSlot.Time = updatedSlot.Time;
            existingSlot.IsBooked = updatedSlot.IsBooked;

            // Replace the existing slot with the updated slot
            var result = await _slots.ReplaceOneAsync(s => s.Id == id, existingSlot);

            if (result.ModifiedCount > 0)
            {
                _logger.LogInformation($"Slot updated: {existingSlot.Date} {existingSlot.Time}");
                return Ok(new { message = "Slot updated successfully" });
            }

            return NotFound(new { message = "Slot not found" });
        }

    }





}
