using MongoDB.Bson;

namespace GP.Ervik.ParticipantManager.Api.DTOs.v1
{
    public class ParticipantCreateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Allergens { get; set; }
        public string? Comment { get; set; }
    }
}
