using Microsoft.AspNetCore.Mvc;
using Lucky.Data;
using Lucky.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lucky.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/Auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == request.Email);

            if (employee == null)
            {
                return Ok(new { message = "If this email exists, a reset link has been sent." });
            }

            var token = Guid.NewGuid().ToString();

            employee.ResetToken = token;
            employee.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Reset token generated successfully.",
                resetToken = token
            });
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == request.Email);

            if (existingEmployee != null)
                return BadRequest(new { message = "Email already registered." });

            var employee = new Employee
            {
                Name = request.Name,
                Email = request.Email,
                Salary = request.Salary,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registered successfully." });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == request.Email);

            if (employee == null || !BCrypt.Net.BCrypt.Verify(request.Password, employee.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            var token = GenerateJwtToken(employee);

            return Ok(new { token });
        }

        private string GenerateJwtToken(Employee employee)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.Email),
                new Claim(JwtRegisteredClaimNames.Email, employee.Email),
                new Claim("employeeId", employee.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }

    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}