using GP.Ervik.ParticipantManager.Data.Models;
using MongoDB.Bson;

namespace GP.Ervik.ParticipantManager.Data.Repositories
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<Participant>> GetAllParticipantsAsync();
        Task<Participant> GetParticipantByIdAsync(ObjectId id);
        Task AddParticipantAsync(Participant participant);
        Task UpdateParticipantAsync(Participant participant);
        Task DeleteParticipantAsync(ObjectId id);
    }

}
