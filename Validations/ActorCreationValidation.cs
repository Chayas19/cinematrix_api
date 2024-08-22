using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class ActorCreationValidation : AbstractValidator<PersonCreationDTO>
    {

        public ActorCreationValidation()
        {

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed more than 100 characters");

            RuleFor(x => x.Picture)
            .NotEmpty().WithMessage("Picture is required ")
            .Must(Validate).WithMessage("Invalid file format");

            RuleFor(x => x.Biography)
                .NotEmpty().WithMessage("Biography is required")
                .MaximumLength(1000).WithMessage("Biography should not exceed more than 1000 characters");

        }

        private bool Validate(IFormFile file)
        {
            return file.Length > 0;
        }
    }
}
