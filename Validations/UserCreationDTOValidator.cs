using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class UserCreationDTOValidator : AbstractValidator<UsercreationDTO>
    {
        public UserCreationDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
            RuleFor(x => x.PhoneNumber)
                       .Must(BeValidPhoneNumber).WithMessage("Phone number must be exactly 10 digits.");
        }

        private bool BeValidPhoneNumber(long phoneNumber)
        {
            var phoneNumberStr = phoneNumber.ToString();
            return phoneNumberStr.Length == 10; // Validate that it has exactly 10 digits
        }
    }
}

