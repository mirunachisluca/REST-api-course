using System;
using System.Collections.Generic;

namespace RestAPI.Models
{
    public class AuthorToInsertDto
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public string MainCategory { get; set; }

        public IEnumerable<CourseToInsertDto> Courses { get; set; } = new List<CourseToInsertDto>();
    }
}
