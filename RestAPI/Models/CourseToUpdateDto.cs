using System.ComponentModel.DataAnnotations;

namespace RestAPI.Models
{
    public class CourseToUpdateDto : CourseToManipulateDto
    {
        [Required(ErrorMessage = "You should fill out a description.")]
        public override string Description { get; set; }
    }
}
