﻿using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class SendOTPValidation : AbstractValidator<OTPVerificationDTO>
    {

        public SendOTPValidation()

        {

            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .NotEqual("string").WithMessage("Email cannot be the placeholder value 'string'.");


            RuleFor(x => x.Code)
              .NotEmpty().WithMessage("Code is required.")
              .NotEqual("string").WithMessage("Code cannot be the placeholder value 'string'.");

            RuleFor(x => x.OtpType)
              .IsInEnum().WithMessage("Invalid OTP type provided.");
        }


    }
}

