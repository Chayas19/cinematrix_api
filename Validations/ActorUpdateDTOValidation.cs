using System.ComponentModel.DataAnnotations;
using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class ActorUpdateDTOValidation : AbstractValidator<PersonUpdateDTO>
    {

        public ActorUpdateDTOValidation()
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

            RuleFor(x => x.DateOfBirth)
      .NotEmpty().WithMessage("Date of birth is required.")
      .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Date of birth must be in yyyy-MM-dd format.");
        }
        private bool Validate(IFormFile file)
        {
            return file.Length > 0;
        }
    }
}
