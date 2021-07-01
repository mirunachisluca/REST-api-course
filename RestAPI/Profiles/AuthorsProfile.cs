using AutoMapper;
using RestAPI.Entities;
using RestAPI.Helpers;
using RestAPI.Models;

namespace RestAPI.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.GetCurrentAge()));

            CreateMap<AuthorToInsertDto, Author>();
        }
    }
}
