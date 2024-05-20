using CarManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;


namespace CarManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IMongoCollection<Appointment> _appointments;
        private readonly IMongoCollection<AvailableSlot> _slots;
        private readonly IMongoCollection<Payment> _payments;

        public AppointmentController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Cars");
            _appointments = database.GetCollection<Appointment>("Appointments");
            _slots = database.GetCollection<AvailableSlot>("AvailableSlots");
            _payments = database.GetCollection<Payment>("Payments");

        }
        [Authorize(Roles = "User")]
        [HttpPost("book")]
        public async Task<IActionResult> BookAppointment([FromBody] Appointment appointment)
        {

            var userId = User.FindFirstValue(ClaimTypes.Actor);
            Console.WriteLine(userId);


            if (userId == null)
            {
                return Unauthorized();
            }

            var slot = await _slots.Find(s => s.Id == appointment.SlotId && !s.IsBooked).FirstOrDefaultAsync();
            if (slot == null)
            {
                return BadRequest(new { message = "Slot not available" });
            }

            slot.IsBooked = true;
            await _slots.ReplaceOneAsync(s => s.Id == slot.Id, slot);

            appointment.UserId = userId;
            appointment.Paid = false; // Initially not paid
            await _appointments.InsertOneAsync(appointment);

            return Ok(new { message = "Appointment booked successfully" });
        }
        [Authorize(Roles = "User")]
        [HttpPost("pay")]
        public async Task<IActionResult> PayForAppointment([FromBody] Payment payment)
        {
            var userId = User.FindFirstValue(ClaimTypes.Actor);

            if (userId == null)
            {
                return Unauthorized();
            }

            payment.UserId = userId;
            payment.Date = DateTime.UtcNow;
            await _payments.InsertOneAsync(payment);

            var appointment = await _appointments.Find(a => a.Id == payment.AppointmentId && a.UserId == userId).FirstOrDefaultAsync();
            if (appointment != null)
            {
                appointment.Paid = true;
                appointment.PaymentId = payment.Id;
                await _appointments.ReplaceOneAsync(a => a.Id == appointment.Id, appointment);
                return Ok(new { message = "Payment successful" });
            }

            return NotFound(new { message = "Appointment not found" });
        }
        [Authorize(Roles = "User")]
        [HttpGet("myAppointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.Actor);

            if (userId == null)
            {
                return Unauthorized();
            }

            var appointments = await _appointments.Find(a => a.UserId == userId).ToListAsync();
            return Ok(appointments);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _appointments.Find(_ => true).ToListAsync();
            return Ok(appointments);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("byType")]
        public async Task<IActionResult> GetAppointmentsByType([FromQuery] string type)
        {
            var appointments = await _appointments.Find(a => a.Type == type).ToListAsync();
            return Ok(appointments);
        }


        [Authorize(Roles = "Employee")]
        [HttpPut("updateStatus/{id}")]
        public async Task<IActionResult> UpdateAppointmentStatus(string id, [FromBody] Appointment updatedAppointment)
        {
            var appointment = await _appointments.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            appointment.Status = updatedAppointment.Status;
            appointment.Details = updatedAppointment.Details;

            await _appointments.ReplaceOneAsync(a => a.Id == id, appointment);
            return Ok(new { message = "Appointment status updated successfully" });
        }
    }
}
