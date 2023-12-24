using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GP.Ervik.ParticipantManager.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/administration")]
    public class AdministrationController : ControllerBase
    {
        private readonly ILogger<AdministrationController> _logger;
        private readonly MongoDbContext _mongoContext;

        public AdministrationController(ILogger<AdministrationController> logger, MongoDbContext context)
        {
            _logger = logger;
            _mongoContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdministrationReadDto>>> Get()
        {
            try
            {
                _logger.LogInformation("Getting list of administrations");

                var response = (await _mongoContext.Administrations.ToListAsync())
                    .Select(admin => new AdministrationReadDto
                    {
                        Id = admin.Id.ToString(),
                        Name = admin.Name,
                        Email = admin.Email,
                    });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving administrations");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}