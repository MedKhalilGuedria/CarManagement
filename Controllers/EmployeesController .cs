using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;


namespace CarManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IMongoClient mongoClient, ILogger<EmployeesController> logger)
        {
            var database = mongoClient.GetDatabase("Cars");
            _users = database.GetCollection<User>("Users");
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetEmployees()
        {
            var employees = await _users.Find(u => u.Role == "Employee").ToListAsync();
            return Ok(employees);
        }

        public async Task<IActionResult> AddEmployee(User employee)
        {
            // Hash the password before storing it
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(employee.Password);
            employee.Password = hashedPassword;

            // Set role to Employee
            employee.Role = "Employee";

            await _users.InsertOneAsync(employee);
            _logger.LogInformation($"Employee '{employee.Username}' added successfully.");
            return Ok(new { message = "Employee added successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(string id, User employee)
        {
            var existingEmployee = await _users.Find(u => u.Id == id && u.Role == "Employee").FirstOrDefaultAsync();
            if (existingEmployee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            employee.Id = id;
            await _users.ReplaceOneAsync(u => u.Id == id, employee);
            _logger.LogInformation($"Employee '{employee.Username}' updated successfully.");
            return Ok(new { message = "Employee updated successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id && u.Role == "Employee");
            if (result.DeletedCount == 0)
            {
                return NotFound(new { message = "Employee not found" });
            }
            _logger.LogInformation($"Employee with ID '{id}' deleted successfully.");
            return Ok(new { message = "Employee deleted successfully" });
        }
    }
}
