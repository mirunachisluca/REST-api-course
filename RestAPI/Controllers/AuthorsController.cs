using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Entities;
using RestAPI.Helpers;
using RestAPI.Models;
using RestAPI.ResourceParams;
using RestAPI.Services;
using System;
using System.Collections.Generic;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

            var previousPageLink = authors.HasPrevious==true
                ? CreateAuthorsResourceUri(authorsParams, ResourceUriType.PreviousPage)
                : null;

            var nextPageLink = authors.HasNext==true
                ? CreateAuthorsResourceUri(authorsParams, ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = authors.TotalCount,
                pageSize = authors.PageSize,
                currentPage = authors.CurrentPage,
                totalPages = authors.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IReadOnlyCollection<AuthorDto>>(authors));
        }

        /// <summary>
        /// Get an author by their id
        /// </summary>
        /// <param name="id">The id of the author</param>
        /// <returns>An ActionResult of type Author</returns>
        /// <response code="200">Returns the requested author</response>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        private string CreateAuthorsResourceUri(AuthorsResourceParams authorsParams, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            pageNumber = authorsParams.PageNumber - 1,
                            pageSize = authorsParams.PageSize,
                            mainCategory = authorsParams.MainCategory,
                            searchQuery = authorsParams.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                        new
                        {
                            pageNumber = authorsParams.PageNumber + 1,
                            pageSize = authorsParams.PageSize,
                            mainCategory = authorsParams.MainCategory,
                            searchQuery = authorsParams.SearchQuery
                        });
                default:
                    return Url.Link("GetAuthors",
                        new
                        {
                            pageNumber = authorsParams.PageNumber,
                            pageSize = authorsParams.PageSize,
                            mainCategory = authorsParams.MainCategory,
                            searchQuery = authorsParams.SearchQuery
                        });
            }
        }
    }
}
