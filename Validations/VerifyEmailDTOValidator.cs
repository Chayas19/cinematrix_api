﻿using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class VerifyEmailDTOValidator : AbstractValidator<EmailVerificationdto>
    {
        public VerifyEmailDTOValidator()
        {


            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .NotEqual("string").WithMessage("Email cannot be the placeholder value 'string'.");




        }


    }
}
