using System;

namespace RestAPI.Models
{
    /// <summary>
    /// An author with id, name, age and category
    /// </summary>
    public class AuthorDto
    {
        /// <summary>
        /// The id of the author
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The author's full name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The author's age
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// The author's book main category
        /// </summary>
        public string MainCategory { get; set; }
    }
}
