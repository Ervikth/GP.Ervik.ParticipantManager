using AutoMapper;
using GP.Ervik.ParticipantManager.Api.DTOs.v1;
using GP.Ervik.ParticipantManager.Data.Models;

namespace GP.Ervik.ParticipantManager.Api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Administration, AdministrationDto>();
            CreateMap<Participant, ParticipantDto>();
            CreateMap<ParticipantDto, Participant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id during update
            CreateMap<ParticipantCreateDto, Participant>();
        }
    }
}
