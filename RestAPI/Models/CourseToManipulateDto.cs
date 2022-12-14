using RestAPI.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace RestAPI.Models
{
    
    [CourseTitleDifferentFromDescription(ErrorMessage = "Title and description cannot be the same.")]
    public abstract class CourseToManipulateDto
    {
        /// <summary>
        /// The name of the course
        /// </summary>
        [Required(ErrorMessage = "You should fill out a title.")]
        [MaxLength(100, ErrorMessage = "Title shouldn't exceed 100 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// Course description
        /// </summary>
        [MaxLength(1500, ErrorMessage = "Description shouldn't exceed 1500 characters.")]
        public virtual string Description { get; set; }
    }
}
