using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Api.Services.Results;
using GP.Ervik.ParticipantManager.Data.Models;

namespace GP.Ervik.ParticipantManager.Api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Administration admin);
        Task<AuthenticationResult> RegisterUser(AdministrationCreateDto administrationCreateDto);
        Task<AuthenticationResult> Login(string username, string password);
    }
}
