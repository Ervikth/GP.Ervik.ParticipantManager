using MongoDB.Bson;

namespace GP.Ervik.ParticipantManager.Data.Models
{
    public class Participant
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Allergens { get; set; }
        public string? Comment { get; set; }
    }
}