using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CarManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace CarManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _jwtSecret;
        private readonly double _jwtExpirationMinutes = 30;
        private readonly IMongoCollection<User> _users;

        public AuthController(IConfiguration configuration, IMongoClient mongoClient)
        {
            _jwtSecret = configuration["Jwt:SecretKey"];
            var database = mongoClient.GetDatabase("Cars");
            _users = database.GetCollection<User>("Users");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var existingUser = await _users.Find(u => u.Username == model.Username).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var user = new User
            {
                Username = model.Username,
                Password = hashedPassword,
                Role =model.Role
            };

            await _users.InsertOneAsync(user);

            return Ok(new { message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _users.Find(u => u.Username == model.Username).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username" });
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid password" });
            }
            var role = user.Role;
            var user_id = user.Id;// Fetch the role from the user object

            var token = GenerateJwtToken(user.Username,role,user_id);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string username, string role,string user_id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Actor, user_id)
             // Include role claim
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
