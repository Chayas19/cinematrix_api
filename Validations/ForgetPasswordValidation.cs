using System.Data;
using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class ForgetPasswordValidation : AbstractValidator<ForgotPasswordDTO>
    {
       
        public ForgetPasswordValidation()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .NotEqual("string").WithMessage("Email cannot be the placeholder value 'string'.");



        }
    }
}
