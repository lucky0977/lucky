using Microsoft.AspNetCore.Mvc;
using Lucky.Data;
using Lucky.Models;
using Microsoft.EntityFrameworkCore;

namespace Lucky.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == request.Email);

            if (employee == null)
            {
                // Security best practice: don't reveal if email exists or not
                return Ok(new { message = "If this email exists, a reset link has been sent." });
            }

            // Generate a simple reset token
            var token = Guid.NewGuid().ToString();

            employee.ResetToken = token;
            employee.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);

            await _context.SaveChangesAsync();

            // In real projects, you'd email this token instead of returning it
            return Ok(new
            {
                message = "Reset token generated successfully.",
                resetToken = token // ⚠️ only for practice/testing, never expose this in production
            });
        }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
}