using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReqresIntegratedApplication.WebAPI.Helpers;
using ReqresIntegratedApplication.WebAPI.Models;
using ReqresIntegratedApplication.WebAPI.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var (token, error) = await _authService.LoginAsync(request.Email, request.Password);
            if (string.IsNullOrWhiteSpace(token))
            {
                var message = string.IsNullOrWhiteSpace(error)
                    ? "Login failed. Please check your email and password."
                    : error;
                return Unauthorized(message);
            }

            return Ok(new { token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _authService.Logout();
            return Ok(new { message = "Logged out" });
        }

        [HttpGet("session")]
        public IActionResult SessionInfo()
        {
            return Ok(new { authenticated = Session.IsAuthenticated, token = Session.CurrentToken });
        }
    }
}
