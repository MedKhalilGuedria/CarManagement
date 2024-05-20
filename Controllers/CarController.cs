using CarManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace CarManagement.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly IMongoCollection<Car> _cars;

        public CarController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Cars");
            _cars = database.GetCollection<Car>("Cars");
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCar([FromBody] Car car)
        {
            await _cars.InsertOneAsync(car);
            return Ok(new { message = "Car added successfully", carId = car.Id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(string id)
        {
            var car = await _cars.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (car == null)
            {
                return NotFound(new { message = "Car not found" });
            }
            return Ok(car);
        }
    }
}
