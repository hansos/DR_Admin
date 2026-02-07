using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DR_Admin_FrontEnd_Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IHttpClientFactory httpClientFactory, ILogger<AccountController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DrAdminApi");
                
                // Map email to username for backend API
                var backendRequest = new { Username = request.Email, Password = request.Password };
                var response = await client.PostAsJsonAsync("/api/v1/Auth/login", backendRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    
                    // Store session if needed
                    if (!string.IsNullOrEmpty(request.Email))
                    {
                        HttpContext.Session.SetString("UserEmail", request.Email);
                    }

                    return Content(result, "application/json");
                }

                return StatusCode((int)response.StatusCode, new 
                { 
                    success = false, 
                    message = "Login failed" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "An error occurred during login" 
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DrAdminApi");
                
                // Map to backend RegisterAccountRequestDto structure
                var backendRequest = new 
                {
                    Username = request.Email.Split('@')[0], // Generate username from email
                    Email = request.Email,
                    Password = request.Password,
                    ConfirmPassword = request.ConfirmPassword,
                    CustomerName = request.Name,
                    CustomerEmail = request.Email,
                    CustomerPhone = "",
                    CustomerAddress = ""
                };
                var response = await client.PostAsJsonAsync("/api/v1/MyAccount/register", backendRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Content(result, "application/json");
                }

                return StatusCode((int)response.StatusCode, new 
                { 
                    success = false, 
                    message = "Registration failed" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "An error occurred during registration" 
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                // Note: The backend API doesn't have a "request password reset" endpoint yet.
                // This would typically send an email with a reset token.
                // For now, return a success message to match frontend expectations.
                
                _logger.LogWarning("Password reset requested for {Email} but backend endpoint not implemented", request.Email);
                
                return Ok(new 
                { 
                    success = true, 
                    message = "If an account exists with this email, you will receive password reset instructions." 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "An error occurred during password reset" 
                });
            }
        }
    }

    // Request models
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
