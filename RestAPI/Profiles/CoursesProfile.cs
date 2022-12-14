using AutoMapper;
using RestAPI.Entities;
using RestAPI.Models;

namespace RestAPI.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Course, CourseDto>();
            CreateMap<CourseToInsertDto, Course>();
            CreateMap<CourseToUpdateDto, Course>();
            CreateMap<Course, CourseToUpdateDto>();
        }
    }
}
