using GP.Ervik.ParticipantManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace GP.Ervik.ParticipantManager.Data.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly MongoDbContext _context;

        public ParticipantRepository(MongoDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Participant>> GetAllParticipantsAsync()
        {
            return await _context.Participants.ToListAsync();
        }

        public async Task<Participant> GetParticipantByIdAsync(ObjectId id)
        {
            return await _context.Participants.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddParticipantAsync(Participant participant)
        {
            participant.Id = ObjectId.GenerateNewId();
            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateParticipantAsync(Participant participant)
        {
            _context.Entry(participant).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteParticipantAsync(ObjectId id)
        {
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.Id == id);
            if (participant != null)
            {
                _context.Participants.Remove(participant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
