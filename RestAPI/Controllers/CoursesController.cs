using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Entities;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/authors/{id:guid}/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICourseLibraryRepository _courseLibraryRepository;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<CourseDto>> GetCoursesForAuthor(Guid id)
        {
            if (!_courseLibraryRepository.AuthorExists(id))
            {
                return NotFound();
            }

            var courses = _courseLibraryRepository.GetCourses(id);

            return Ok(_mapper.Map<IReadOnlyList<CourseDto>>(courses));
        }

        [HttpGet("{courseId:guid}", Name = "GetCourseForAuthor")]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid id, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(id))
            {
                return NotFound();
            }

            var course = _courseLibraryRepository.GetCourse(id, courseId);

            if (course == null) return NotFound();

            return _mapper.Map<CourseDto>(course);
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourseForAuthor(Guid id, CourseToInsertDto courseDto)
        {
            if (!_courseLibraryRepository.AuthorExists(id)) return NotFound();

            var course = _mapper.Map<Course>(courseDto);

            _courseLibraryRepository.AddCourse(id, course);
            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<CourseDto>(course);

            return CreatedAtRoute("GetCourseForAuthor", new { id, courseId = courseToReturn.Id }, courseToReturn);
        }

        [HttpPut("{courseId:guid}")]
        public IActionResult UpdateCourseForAuthor(Guid id, Guid courseId, CourseToUpdateDto courseDto)
        {
            if (!_courseLibraryRepository.AuthorExists(id)) return NotFound();

            var course = _courseLibraryRepository.GetCourse(id, courseId);

            if (course == null)
            {
                var newCourse = _mapper.Map<Course>(courseDto);
                newCourse.Id = courseId;

                _courseLibraryRepository.AddCourse(id, newCourse);
                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CourseDto>(newCourse);

                return CreatedAtRoute("GetCourseForAuthor", new { id, courseId = courseToReturn.Id }, courseToReturn);
            }

            _mapper.Map(courseDto, course);

            _courseLibraryRepository.UpdateCourse(course);
            _courseLibraryRepository.Save();

            return NoContent();
        }
    }
}
