using MongoDB.Bson;

namespace GP.Ervik.ParticipantManager.Data.Models
{
    public class Administration
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}