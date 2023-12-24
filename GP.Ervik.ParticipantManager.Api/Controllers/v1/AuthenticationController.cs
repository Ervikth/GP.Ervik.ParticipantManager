using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GP.Ervik.ParticipantManager.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authService;
        private readonly ILogger<AdministrationController> _logger;

        public AuthenticationController(ILogger<AdministrationController> logger, AuthenticationService authService)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Email and password are required.");
            }

            try
            {
                var authResult = await _authService.Login(email, password);

                if (authResult.IsAuthenticated)
                {
                    return Ok(new { Token = authResult.Token });
                }

                return Unauthorized("Invalid email or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AdministrationCreateDto administrationCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var registrationResult = await _authService.RegisterUser(administrationCreateDto);

                if (registrationResult.IsAuthenticated)
                {
                    return Ok(new { Token = registrationResult.Token });
                }

                return BadRequest("Registration failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for email.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


    }
}
