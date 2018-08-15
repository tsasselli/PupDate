using AutoMapper;
using PupDate.API.Dtos;
using PupDate.API.Models;

namespace PupDate.API.helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>();
            CreateMap<User, UserForDetailedDto>();
        }
    }
}