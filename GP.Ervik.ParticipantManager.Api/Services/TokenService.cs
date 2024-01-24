using GP.Ervik.ParticipantManager.Api.Controllers.v1;
using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Api.Interfaces;
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
    public class TokenService : ITokenService
    {
        private readonly MongoDbContext _mongoContext;
        private readonly ILogger<ParticipantController> _logger;
        private readonly SymmetricSecurityKey _key;

        public TokenService(ILogger<ParticipantController> logger, MongoDbContext mongoContext,
            IConfiguration config)
        {
            _mongoContext = mongoContext;
            _logger = logger;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(Administration admin)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, admin.Username)
            };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
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
    }
}