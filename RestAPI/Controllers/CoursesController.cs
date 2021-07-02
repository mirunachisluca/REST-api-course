using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        [HttpPatch("{courseId:guid}")]
        public ActionResult PartiallyUpdateCourseForAuthor(Guid id, Guid courseId,
            JsonPatchDocument<CourseToUpdateDto> patchDocument)
        {
            if (!_courseLibraryRepository.AuthorExists(id)) return NotFound();

            var course = _courseLibraryRepository.GetCourse(id, courseId);

            if (course == null)
            {
                var courseDto = new CourseToUpdateDto();
                patchDocument.ApplyTo(courseDto, ModelState);

                if (!TryValidateModel(courseDto)) return ValidationProblem(ModelState);


                var newCourse = _mapper.Map<Course>(courseDto);
                newCourse.Id = courseId;

                _courseLibraryRepository.AddCourse(id, newCourse);
                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CourseDto>(newCourse);

                return CreatedAtRoute("GetCourseForAuthor", new { id, courseId = courseToReturn.Id }, courseToReturn);
            }

            var courseToUpdate = _mapper.Map<CourseToUpdateDto>(course);

            patchDocument.ApplyTo(courseToUpdate, ModelState);

            if (!TryValidateModel(courseToUpdate))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(courseToUpdate, course);

            _courseLibraryRepository.UpdateCourse(course);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpDelete("{courseId:guid}")]
        public ActionResult DeleteCourseForAuthor(Guid id, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(id)) return NotFound();

            var course = _courseLibraryRepository.GetCourse(id, courseId);

            if (course == null) return NotFound();

            _courseLibraryRepository.DeleteCourse(course);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
