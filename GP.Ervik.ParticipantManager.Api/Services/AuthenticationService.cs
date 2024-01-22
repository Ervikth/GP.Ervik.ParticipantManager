using GP.Ervik.ParticipantManager.Api.Controllers.v1;
using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Api.Services.Results;
using GP.Ervik.ParticipantManager.Data;
using GP.Ervik.ParticipantManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GP.Ervik.ParticipantManager.Api.Services
{
    public class AuthenticationService
    {
        private readonly MongoDbContext _mongoContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ParticipantController> _logger;

        public AuthenticationService(ILogger<ParticipantController> logger, MongoDbContext mongoContext,
            IConfiguration configuration)
        {
            _mongoContext = mongoContext;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthenticationResult> RegisterUser(AdministrationCreateDto administrationCreateDto)
        {
            try
            {
                _logger.LogInformation("Attempting to register user with username");

                var existingAdmin =
                    await _mongoContext.Administrations.AnyAsync(a => a.Username == administrationCreateDto.Username);
                if (existingAdmin)
                {
                    _logger.LogWarning("Registration failed: Username already exists");
                    return new AuthenticationResult { IsAuthenticated = false, Token = null };
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(administrationCreateDto.Password);
                var admin = new Administration
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = administrationCreateDto.Name,
                    Username = administrationCreateDto.Username,
                    Password = passwordHash,
                };

                await _mongoContext.Administrations.AddAsync(admin);
                await _mongoContext.SaveChangesAsync();

                _logger.LogInformation("User registered successfully with username");

                string token = CreateToken(admin);
                return new AuthenticationResult
                {
                    IsAuthenticated = true,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration with username");
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    Token = null
                };
            }
        }

        public async Task<AuthenticationResult> Login(string username, string password)
        {
            try
            {
                _logger.LogInformation("Attempting login for user with username");

                var admin = await _mongoContext.Administrations.SingleOrDefaultAsync(p => p.Username == username);

                if (admin == null)
                {
                    _logger.LogWarning("Login failed: User not found for username");
                    return new AuthenticationResult { IsAuthenticated = false, Token = null };
                }

                if (!BCrypt.Net.BCrypt.Verify(password, admin.Password))
                {
                    _logger.LogWarning("Login failed: Incorrect password for username");
                    return new AuthenticationResult { IsAuthenticated = false, Token = null };
                }

                _logger.LogInformation("User logged in successfully with username");

                // User is authenticated, create token
                string token = CreateToken(admin);
                return new AuthenticationResult { IsAuthenticated = true, Token = token };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for username");
                return new AuthenticationResult { IsAuthenticated = false, Token = null };
            }
        }

        private string CreateToken(Administration admin)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Username)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Key").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}