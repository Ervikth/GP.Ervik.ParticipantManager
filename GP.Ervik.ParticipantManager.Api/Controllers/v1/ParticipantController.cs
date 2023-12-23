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
            return Ok(response);
        }

        [HttpGet("userId"), Authorize]
        public async Task<IActionResult> GetParticipant(string userId)
        {
            var user = await _mongoContext.Participants.FindAsync(ObjectId.Parse(userId));

            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var userDto = new ParticipantDto
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Allergens = user.Allergens,
                Comment = user.Comment
            };
            return Ok(userDto);
        }
        [HttpPost, Authorize]
        public async Task<IActionResult> AddParticipant(ParticipantCreateDto participantCreateDto)
        {
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

            return CreatedAtAction(nameof(GetParticipant), new { id = participant.Id }, participant);
        }
        [HttpPatch("{userId}"), Authorize]
        public async Task<IActionResult> UpdateParticipant(string userId, ParticipantDto participantDto)
        {
            var participant = await _mongoContext.Participants.FindAsync(ObjectId.Parse(userId));
            if (participant == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            if (participantDto != null) participant.Name = participantDto.Name;
            if (participantDto.Email != null) participant.Email = participantDto.Email;
            if (participantDto.PhoneNumber != null) participant.PhoneNumber = participantDto.PhoneNumber;
            if (participantDto.Allergens != null) participant.Allergens = participantDto.Allergens;
            if (participantDto.Comment != null) participant.Comment = participantDto.Comment;
            _mongoContext.Participants.Update(participant);
            await _mongoContext.SaveChangesAsync();

            return Ok(participant);
        }
        [HttpDelete("{userId}"), Authorize]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _mongoContext.Participants.FindAsync(ObjectId.Parse(userId));
            if (user == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            _mongoContext.Participants.Remove(user);
            await _mongoContext.SaveChangesAsync();
            return NoContent();
        }

    }
}
