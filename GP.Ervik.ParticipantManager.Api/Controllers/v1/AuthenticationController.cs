using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GP.Ervik.ParticipantManager.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AdministrationController> _logger;


        public AuthenticationController(ILogger<AdministrationController> logger, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _logger = logger;
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
                var registrationResult = await _tokenService.RegisterUser(administrationCreateDto);

                if (registrationResult.IsAuthenticated)
                {
                    return Ok(new { Token = registrationResult.Token });
                }

                return BadRequest("Registration failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for username.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            try
            {
                var authResult = await _tokenService.Login(username, password);

                if (authResult.IsAuthenticated)
                {
                    return Ok(new { Token = authResult.Token });
                }

                return Unauthorized("Invalid username or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for username.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


    }
}