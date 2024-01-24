using AutoMapper;
using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Data.Models;
using GP.Ervik.ParticipantManager.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GP.Ervik.ParticipantManager.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/participant")]
    public class ParticipantController : ControllerBase
    {
        private readonly ILogger<ParticipantController> _logger;
        private readonly IParticipantRepository _participantRepository;
        private readonly IMapper _mapper;

        public ParticipantController(ILogger<ParticipantController> logger, IParticipantRepository participantRepository, IMapper mapper)
        {
            _logger = logger;
            _participantRepository = participantRepository;
            _mapper = mapper;
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> Get()
        {
            try
            {
                _logger.LogInformation("Retrieving all participants");

                var participants = await _participantRepository.GetAllParticipantsAsync();
                var participantsToReturn = _mapper.Map<IEnumerable<ParticipantDto>>(participants);

                _logger.LogInformation("Retrieved participants");

                return Ok(participantsToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving participants");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{Id}"), Authorize]
        public async Task<IActionResult> GetParticipant(string Id)
        {
            try
            {
                _logger.LogInformation("Getting participant");

                var participant = await _participantRepository.GetParticipantByIdAsync(ObjectId.Parse(Id));

                if (participant == null)
                {
                    _logger.LogWarning("Participant with ID not found.");
                    return NotFound("Participant with ID not found.");
                }

                var participantToReturn = _mapper.Map<ParticipantDto>(participant);

                _logger.LogInformation("Participant retrieved successfully");
                return Ok(participantToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving participant with ID.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<ParticipantDto>> AddParticipant(ParticipantCreateDto participantCreateDto)
        {
            try
            {
                _logger.LogInformation("Adding new participant.");

                var participant = _mapper.Map<Participant>(participantCreateDto);
                await _participantRepository.AddParticipantAsync(participant);

                _logger.LogInformation("Participant added successfully");

                var participantDto = _mapper.Map<ParticipantDto>(participant);
                return CreatedAtAction(nameof(GetParticipant), new { Id = participantDto.Id.ToString() }, participantDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when adding participant.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPatch("{participantId}"), Authorize]
        public async Task<IActionResult> UpdateParticipant(string participantId, ParticipantDto participantDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update participant.");
                var participant = await _participantRepository.GetParticipantByIdAsync(ObjectId.Parse(participantId));
                if (participant == null)
                {
                    _logger.LogWarning("Participant not found - ID: {ParticipantId}", participantId);
                    return NotFound($"Participant with ID {participantId} not found.");
                }

                _mapper.Map(participantDto, participant);

                await _participantRepository.UpdateParticipantAsync(participant);

                var updatedParticipantDto = _mapper.Map<ParticipantDto>(participant);

                _logger.LogInformation("Participant updated successfully - ID: {ParticipantId}", participantId);

                return Ok(updatedParticipantDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating participant - ID: {ParticipantId}", participantId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{participantId}"), Authorize]
        public async Task<IActionResult> DeleteParticipant(string participantId)
        {
            try
            {
                _logger.LogInformation("Trying to delete participant - ID: {ParticipantId}", participantId);
                var participant = await _participantRepository.GetParticipantByIdAsync(ObjectId.Parse(participantId));
                if (participant == null)
                {
                    _logger.LogWarning("Participant not found - ID: {ParticipantId}", participantId);
                    return NotFound($"Participant with ID {participantId} not found.");
                }

                await _participantRepository.DeleteParticipantAsync(ObjectId.Parse(participantId));

                _logger.LogInformation("Participant deleted successfully - ID: {ParticipantId}", participantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting participant - ID: {ParticipantId}", participantId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}