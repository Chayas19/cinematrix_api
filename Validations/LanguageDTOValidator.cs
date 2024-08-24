using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class LanguageDTOValidator : AbstractValidator<LanguageDTO>
    {
        public LanguageDTOValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Name is required.")
               .Length(1, 50).WithMessage("Name must be between 1 and 50 characters.");
        }
    }
}
