using RestAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RestAPI.ValidationAttributes
{
    public class CourseTitleDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseToManipulateDto)validationContext.ObjectInstance;

            if (course.Title == course.Description)
            {
                return new ValidationResult(ErrorMessage,
                    new[] { nameof(CourseToManipulateDto) });
            }

            return ValidationResult.Success;
        }
    }
}