using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class GenreCreationValidation : AbstractValidator<GenreCreationDTO>
    {
        public GenreCreationValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.")
                .Matches("^[a-zA-Z ]*$").WithMessage("Name must contain only alphabetic characters and spaces.");
        }
    }

}
