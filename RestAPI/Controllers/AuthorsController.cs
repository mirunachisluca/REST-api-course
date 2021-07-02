using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services;
using System;
using System.Collections.Generic;
using RestAPI.Entities;
using RestAPI.ResourceParams;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [HttpHead]
        public ActionResult<IReadOnlyCollection<AuthorDto>> GetAuthors([FromQuery] AuthorsResourceParams authorsParams)
        {
            var authors = _courseLibraryRepository.GetAuthors(authorsParams);

            return Ok(_mapper.Map<IReadOnlyCollection<AuthorDto>>(authors));
        }

        [HttpGet("{id:guid}", Name = "GetAuthor")]
        public ActionResult<AuthorDto> GetAuthor(Guid id)
        {
            var author = _courseLibraryRepository.GetAuthor(id);

            if (author == null) return NotFound();

            return Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorToInsertDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);

            _courseLibraryRepository.AddAuthor(author);
            _courseLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(author);

            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public ActionResult DeleteAuthor(Guid id)
        {
            var author = _courseLibraryRepository.GetAuthor(id);

            if (author == null) return NotFound();

            _courseLibraryRepository.DeleteAuthor(author);
            _courseLibraryRepository.Save();

            return NoContent();
        }
    }
}
