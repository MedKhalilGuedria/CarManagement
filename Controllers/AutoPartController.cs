using CarManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AutoPartController : ControllerBase
    {
        private readonly IMongoCollection<AutoPart> _autoParts;

        public AutoPartController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Cars");
            _autoParts = database.GetCollection<AutoPart>("AutoParts");
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAutoPart([FromBody] AutoPart autoPart)
        {
            await _autoParts.InsertOneAsync(autoPart);
            return Ok(new { message = "Auto part added successfully", autoPartId = autoPart.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAutoParts()
        {
            var autoParts = await _autoParts.Find(_ => true).ToListAsync();
            return Ok(autoParts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAutoPart(string id)
        {
            var autoPart = await _autoParts.Find(ap => ap.Id == id).FirstOrDefaultAsync();
            if (autoPart == null)
            {
                return NotFound(new { message = "Auto part not found" });
            }
            return Ok(autoPart);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAutoPart(string id, [FromBody] AutoPart updatedAutoPart)
        {
            var autoPart = await _autoParts.Find(ap => ap.Id == id).FirstOrDefaultAsync();
            if (autoPart == null)
            {
                return NotFound(new { message = "Auto part not found" });
            }

            autoPart.Name = updatedAutoPart.Name;
            autoPart.Manufacturer = updatedAutoPart.Manufacturer;
            autoPart.Price = updatedAutoPart.Price;
            autoPart.Quantity = updatedAutoPart.Quantity;

            await _autoParts.ReplaceOneAsync(ap => ap.Id == id, autoPart);
            return Ok(new { message = "Auto part updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAutoPart(string id)
        {
            var autoPart = await _autoParts.Find(ap => ap.Id == id).FirstOrDefaultAsync();
            if (autoPart == null)
            {
                return NotFound(new { message = "Auto part not found" });
            }

            await _autoParts.DeleteOneAsync(ap => ap.Id == id);
            return Ok(new { message = "Auto part deleted successfully" });
        }
    }
}
