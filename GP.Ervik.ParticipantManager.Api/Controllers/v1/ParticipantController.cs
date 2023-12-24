using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Data;
using GP.Ervik.ParticipantManager.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace GP.Ervik.ParticipantManager.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/participant")]
    public class ParticipantController : ControllerBase
    {
        private readonly ILogger<ParticipantController> _logger;
        private readonly MongoDbContext _mongoContext;

        public ParticipantController(ILogger<ParticipantController> logger, MongoDbContext context)
        {
            _logger = logger;
            _mongoContext = context;
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<List<ParticipantDto>>> Get()
        {
            try
            {
                _logger.LogInformation("Retrieving all participants");

                var response = (await _mongoContext.Participants.ToListAsync())
                    .Select(part => new ParticipantDto
                    {
                        Id = part.Id.ToString(),
                        Name = part.Name,
                        Email = part.Email,
                        PhoneNumber = part.PhoneNumber,
                        Allergens = part.Allergens,
                        Comment = part.Comment
                    });
                _logger.LogInformation("Retrieved participants");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving participants");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{participantId}"), Authorize]
        public async Task<IActionResult> GetParticipant(string participantId)
        {
            try
            {
                _logger.LogInformation("Getting participant");

                var participant = await _mongoContext.Participants.FindAsync(ObjectId.Parse(participantId));

                if (participant == null)
                {
                    _logger.LogWarning("Participant with ID not found.");
                    return NotFound("Participant with ID not found.");
                }

                var participantDto = new ParticipantDto
                {
                    Id = participant.Id.ToString(),
                    Name = participant.Name,
                    Email = participant.Email,
                    PhoneNumber = participant.PhoneNumber,
                    Allergens = participant.Allergens,
                    Comment = participant.Comment
                };

                _logger.LogInformation("Participant retrieved successfully");
                return Ok(participantDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving participant with ID.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<ParticipantCreateDto>> AddParticipant(ParticipantCreateDto participantCreateDto)
        {
            try
            {
                _logger.LogInformation("Adding new participant.");

                var participant = new Participant
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = participantCreateDto.Name,
                    Email = participantCreateDto.Email,
                    PhoneNumber = participantCreateDto.PhoneNumber,
                    Allergens = participantCreateDto.Allergens,
                    Comment = participantCreateDto.Comment
                };

                await _mongoContext.Participants.AddAsync(participant);
                await _mongoContext.SaveChangesAsync();

                _logger.LogInformation("Participant added successfully");

                return CreatedAtAction(nameof(GetParticipant), new { participantId = participant.Id.ToString() },
                    participant);
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
                var participant = await _mongoContext.Participants.FindAsync(ObjectId.Parse(participantId));
                if (participant == null)
                {
                    _logger.LogWarning("Participant not found - ID: {ParticipantId}", participantId);
                    return NotFound($"Participant with ID {participantId} not found.");
                }

                if (participantDto != null) participant.Name = participantDto.Name;
                if (participantDto.Email != null) participant.Email = participantDto.Email;
                if (participantDto.PhoneNumber != null) participant.PhoneNumber = participantDto.PhoneNumber;
                if (participantDto.Allergens != null) participant.Allergens = participantDto.Allergens;
                if (participantDto.Comment != null) participant.Comment = participantDto.Comment;

                _mongoContext.Participants.Update(participant);
                await _mongoContext.SaveChangesAsync();

                _logger.LogInformation("Participant updated successfully - ID: {ParticipantId}", participantId);

                return Ok(participant);
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
                var participant = await _mongoContext.Participants.FindAsync(ObjectId.Parse(participantId));
                if (participant == null)
                {
                    _logger.LogWarning("Participant not found - ID: {ParticipantId}", participantId);
                    return NotFound($"Participant with ID {participantId} not found.");
                }

                _mongoContext.Participants.Remove(participant);
                await _mongoContext.SaveChangesAsync();

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