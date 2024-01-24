using GP.Ervik.ParticipantManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace GP.Ervik.ParticipantManager.Data.Repositories
{
    public class AdministrationRepository : IAdministrationRepository
    {

        private readonly MongoDbContext _context;
        public AdministrationRepository(MongoDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Administration>> GetAllAdministrationsAsync()
        {
            return await _context.Administrations.ToListAsync();
        }

        public async Task<Administration> GetAdministrationByIdAsync(ObjectId id)
        {
            return await _context.Administrations.FindAsync(id);
        }

        public async Task AddAdministrationAsync(Administration administration)
        {
            _context.Administrations.Add(administration);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAdministrationAsync(Administration administration)
        {
            _context.Entry(administration).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAdministrationAsync(ObjectId id)
        {
            var administration = await _context.Administrations.FindAsync(id);
            if (administration != null)
            {
                _context.Administrations.Remove(administration);
                await _context.SaveChangesAsync();
            }
        }
    }
}
