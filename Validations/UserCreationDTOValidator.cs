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
                 .Equal(x => x.Password).WithMessage("Passwords do not match.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[\W_]").WithMessage("Password must contain atleast one special character ")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");


            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
              


            RuleFor(x => x.PhoneNumber)
                  .NotEmpty().WithMessage("Phone number is required");
               

        }

        private bool BeValidPhoneNumber(long phoneNumber)
        {
            var phoneNumberStr = phoneNumber.ToString();
            return phoneNumberStr.Length == 10;
        }
    }
}

