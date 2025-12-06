using BCrypt.Net;
using SmartParking.Core.Entities;
using SmartParking.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartParkingSystem;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using SmartParkingSystem.DTOs;

namespace SmartParkingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ParkingContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthController(ParkingContext context, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        // ========================= REGISTER USER =========================

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest req)
        {
            if (await _context.Users.AnyAsync(u => u.Email == req.Email))
                return Conflict(new { statusCode = 409, success = false, message = "Email already registered." });

            if (string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { statusCode = 400, success = false, message = "Password is required." });

            if (!Regex.IsMatch(req.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest(new { statusCode = 400, success = false, message = "Invalid email format." });

            if (!Regex.IsMatch(req.PhoneNumber, @"^(\+?\d{1,3}[- ]?)?\d{10}$"))
                return BadRequest(new { statusCode = 400, success = false, message = "Invalid phone number format." });

            string hashed = BCrypt.Net.BCrypt.HashPassword(req.Password, 12);

            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                Password = hashed,
                PhoneNumber = req.PhoneNumber,
                Role = "User"
            };

            await _context.Users.AddAsync(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500, new { statusCode = 500, success = false, message = "Error saving user to database." });
            }

            SendWelcomeEmail(user);

            return StatusCode(201, new { statusCode = 201, success = true, message = "User registered successfully. A welcome email has been sent." });
        }

        // ========================= REGISTER ADMIN =========================

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest req)
        {
            if (await _context.Users.AnyAsync(u => u.Email == req.Email))
                return Conflict(new { statusCode = 409, success = false, message = "Email already registered." });

            if (string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { statusCode = 400, success = false, message = "Password is required." });

            string hashed = BCrypt.Net.BCrypt.HashPassword(req.Password, 12);

            var admin = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                Password = hashed,
                PhoneNumber = req.PhoneNumber,
                Role = "Admin"
            };

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();

            SendWelcomeEmail(admin);

            return StatusCode(201, new { statusCode = 201, success = true, message = "Admin created successfully." });
        }

        // ========================= EMAIL TEMPLATE =========================

        private void SendWelcomeEmail(User user)
        {
            _ = Task.Run(() =>
            {
                var subject = $"Welcome to Sayes, {user.FullName}!";

                var body = $@"
                <div style='font-family:Segoe UI,Arial,sans-serif;background:#f4f7fb;padding:35px;border-radius:10px;'>
                  <div style='background:#173C65;padding:25px;border-radius:10px 10px 0 0;text-align:center;'>
                    <h1 style='color:#fff;margin:0;font-size:28px;'>Welcome to Sayes</h1>
                  </div>
                  <div style='background:#fff;padding:30px;border-radius:0 0 10px 10px;box-shadow:0 4px 10px rgba(0,0,0,0.08)'>
                    <p style='color:#506C99;font-size:16px;margin-bottom:25px;'>
                      Hi <b>{user.FullName}</b>,<br><br>
                      We're thrilled to have you join <b>Smart Sayes</b>!
                    </p>
                    <div style='text-align:center;margin:35px 0;'>
                        <a href='https://smartpark-website-react.vercel.app/' 
                           style='background:#F6DD55;color:#173C65;padding:12px 28px;
                           border-radius:8px;text-decoration:none;font-weight:600;font-size:16px;
                           box-shadow:0 2px 6px rgba(0,0,0,0.1);'>
                           Get Started
                        </a>
                    </div>
                    <p style='color:#506C99;font-size:14px;line-height:1.6;margin-top:10px'>
                      Your account is now active. You can sign in using your email: <b>{user.Email}</b>.
                    </p>
                  </div>
                </div>";

                _emailService.SendEmail(user.Email, subject, body);
            });
        }




        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

            if (existingUser == null)
                return Unauthorized(new { statusCode = 401, success = false, message = "Invalid email or password." });

            if (string.IsNullOrEmpty(existingUser.Password))
                return Unauthorized(new { statusCode = 401, success = false, message = "Account registered via Google. Use Google login." });

            bool verified = false;

            if (existingUser.Password.StartsWith("$2a$") ||
                existingUser.Password.StartsWith("$2b$") ||
                existingUser.Password.StartsWith("$2y$"))
            {
                verified = BCrypt.Net.BCrypt.Verify(req.Password ?? "", existingUser.Password);
            }
            else if (existingUser.Password == req.Password)
            {
                verified = true;
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(req.Password, 12);
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
            }

            if (!verified)
                return Unauthorized(new { statusCode = 401, success = false, message = "Invalid email or password." });

            var token = GenerateJwtToken(existingUser);
            return Ok(new { statusCode = 200, success = true, message = "Login successful.", data = new { token } });
        }

        // ================= GOOGLE LOGIN =================
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

                bool isNewUser = false;

                if (user == null)
                {
                    isNewUser = true;
                    user = new User
                    {
                        FullName = payload.Name,
                        Email = payload.Email,
                        Password = null
                    };
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                }

                var jwt = GenerateJwtToken(user);

                if (isNewUser)
                {
                    _ = Task.Run(() =>
                    {
                        var subject = "Welcome to Sayes!";
                        var body = $@"
                        <div style='font-family:Segoe UI,Arial,sans-serif;background:#f4f7fb;padding:35px;border-radius:10px;'>
                          <div style='background:#173C65;padding:25px;border-radius:10px 10px 0 0;text-align:center;'>
                            <h1 style='color:#fff;margin:0;font-size:28px;'>Welcome to Sayes</h1>
                          </div>
                          <div style='background:#fff;padding:30px;border-radius:0 0 10px 10px;box-shadow:0 4px 10px rgba(0,0,0,0.08)'>
                            <p style='color:#506C99;font-size:16px;margin-bottom:25px;'>
                              Hi <b>{user.FullName}</b>,<br><br>
                              Thanks for signing in with Google! Your Sayes account has been created successfully.
                            </p>
                          </div>
                        </div>";
                        _emailService.SendEmail(user.Email, subject, body);
                    });
                }

                return isNewUser
                    ? StatusCode(201, new { statusCode = 201, success = true, message = "Google account registered successfully.", data = new { token = jwt } })
                    : Ok(new { statusCode = 200, success = true, message = "Google login successful.", data = new { token = jwt } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { statusCode = 400, success = false, message = $"Invalid Google token: {ex.Message}" });
            }
        }

        // ================= FORGOT PASSWORD =================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null)
                return NotFound(new { statusCode = 404, success = false, message = "Email not registered." });

            user.ResetToken = GenerateSecureToken();
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var resetLink = $"https://your-domain/reset-password.html?email={user.Email}&token={user.ResetToken}";

            var subject = "🔐 Reset Your Sayes Password";
            var body = $@"
                <div style='font-family:Segoe UI,Arial,sans-serif;background:#f4f7fb;padding:35px;border-radius:10px;'>
                  <div style='background:#173C65;padding:25px;border-radius:10px 10px 0 0;text-align:center;'>
                    <h1 style='color:#fff;margin:0;font-size:26px;'>Reset Your Password</h1>
                  </div>
                  <div style='background:#fff;padding:30px;border-radius:0 0 10px 10px;box-shadow:0 4px 10px rgba(0,0,0,0.08)'>
                    <p>Hi <b>{user.FullName}</b>,</p>
                    <p>We received a request to reset your password. Click below to reset it:</p>
                    <div style='text-align:center;margin:35px 0;'>
                        <a href='{resetLink}' style='background:#F6DD55;color:#173C65;padding:12px 28px;border-radius:8px;text-decoration:none;font-weight:600;font-size:16px;'>Reset Password</a>
                    </div>
                    <p>This link will expire in 15 minutes.</p>
                  </div>
                </div>";

            _ = Task.Run(() => _emailService.SendEmail(user.Email, subject, body));

            return Ok(new { statusCode = 200, success = true, message = "Password reset email sent successfully." });
        }

        // ================= RESET PASSWORD =================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null)
                return NotFound(new { statusCode = 404, success = false, message = "User not found." });

            if (user.ResetToken != req.Token || user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
                return BadRequest(new { statusCode = 400, success = false, message = "Invalid or expired token." });

            if (BCrypt.Net.BCrypt.Verify(req.NewPassword, user.Password))
                return BadRequest(new { statusCode = 400, success = false, isCurrentPassword = true, message = "Cannot reuse current password." });

            user.Password = BCrypt.Net.BCrypt.HashPassword(req.NewPassword, 12);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, success = true, message = "Password reset successfully." });
        }

        // ================= JWT GENERATION =================
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtConfig:Issuer"],
                audience: _configuration["JwtConfig:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ================= SECURE TOKEN =================
        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

    // ================= REQUEST MODELS =================
    public class ForgotPasswordRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class GoogleLoginRequest
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; } = string.Empty;
    }
}

