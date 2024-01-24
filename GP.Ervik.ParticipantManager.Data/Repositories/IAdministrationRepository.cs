using GP.Ervik.ParticipantManager.Data.Models;
using MongoDB.Bson;

namespace GP.Ervik.ParticipantManager.Data.Repositories
{
    public interface IAdministrationRepository
    {
        Task<IEnumerable<Administration>> GetAllAdministrationsAsync();
        Task<Administration> GetAdministrationByIdAsync(ObjectId id);
        Task AddAdministrationAsync(Administration administration);
        Task UpdateAdministrationAsync(Administration administration);
        Task DeleteAdministrationAsync(ObjectId id);
    }

}
