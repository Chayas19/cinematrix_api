using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class SubscribeDTOValidation : AbstractValidator<subscribecreationdto>
    {
        public SubscribeDTOValidation()
        {
            RuleFor(dto => dto.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10}$").WithMessage("Phone number must be exactly 10 digits.");
        }
    }
}
