using AutoMapper;
using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GP.Ervik.ParticipantManager.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/administration")]
    public class AdministrationController : ControllerBase
    {
        private readonly ILogger<AdministrationController> _logger;

        private readonly IAdministrationRepository _administrationRepository;
        private readonly IMapper _mapper;

        public AdministrationController(ILogger<AdministrationController> logger, IAdministrationRepository administrationRepository, IMapper mapper)
        {
            _logger = logger;
            _administrationRepository = administrationRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdministrationReadDto>>> Get()
        {
            try
            {
                _logger.LogInformation("Getting list of administrations");
                var administrations = await _administrationRepository.GetAllAdministrationsAsync();
                var administrationsToReturn = _mapper.Map<IEnumerable<AdministrationDto>>(administrations);

                return Ok(administrationsToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving administrations");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}