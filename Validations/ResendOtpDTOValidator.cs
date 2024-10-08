﻿using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class ResendOtpDTOValidator : AbstractValidator<ResendOtpDTO>
    {
        public ResendOtpDTOValidator()
        {

            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .NotEqual("string").WithMessage("Email cannot be the placeholder value 'string'.");
        }

    }
}
