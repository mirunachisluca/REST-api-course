using System.ComponentModel.DataAnnotations;

namespace RestAPI.Models
{
    /// <summary>
    /// Course for update with title and description fields
    /// </summary>
    public class CourseToUpdateDto : CourseToManipulateDto
    {
        /// <summary>
        /// Course description
        /// </summary>
        [Required(ErrorMessage = "You should fill out a description.")]
        public override string Description { get; set; }
    }
}
