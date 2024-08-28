using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AutoMapper;
using CineMatrix_API.DTOs;
using CineMatrix_API.Enums;
using CineMatrix_API.Models;
using CineMatrix_API.Repository;
using CineMatrix_API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Swashbuckle.AspNetCore.Annotations;

namespace CineMatrix_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ISMSService _smsService;
        private readonly IEmailService _emailSender;
        private readonly IPasswordService _passwordService;
        private readonly Ijwtservice _jwtService;
        private readonly IOtpService _otpService;
        private readonly IValidator<UsercreationDTO> _validator;

        public UserAccountController(
            IMapper mapper,
            ApplicationDbContext context,
            IOtpService otpservice,
            ISMSService smsService,
            IEmailService emailSender,
            IPasswordService passwordService,
            Ijwtservice jwtService,
            IValidator<UsercreationDTO> validator
            )
        {
            _mapper = mapper;
            _context = context;
            _otpService = otpservice;   
            _smsService = smsService;
            _emailSender = emailSender;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _validator = validator;


        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsercreationDTO userCreationDto)
        {
            if (userCreationDto == null)
            {
                return BadRequest("User registration data is not valid.");
            }

            if (string.IsNullOrEmpty(userCreationDto.Email) ||
                string.IsNullOrEmpty(userCreationDto.Password)||
                string.IsNullOrEmpty(userCreationDto.ConfirmPassword)||
                string.IsNullOrEmpty(userCreationDto.Name)||
                string.IsNullOrEmpty(userCreationDto.PhoneNumber.ToString()))
            {
                return BadRequest("Fields cannot be empty, Please provide the valid input data");
            }
            
            if (string.IsNullOrEmpty(userCreationDto.Password) || userCreationDto.Password != userCreationDto.ConfirmPassword)
            {
                return BadRequest("Password fields cannot be empty and must match.");
            }

            if (userCreationDto.Email == "string" ||
             userCreationDto.Password == "string" ||
            userCreationDto.ConfirmPassword == "string" ||
            userCreationDto.PhoneNumber < 0 ||
            userCreationDto.Name == "string")
            {
                return BadRequest("Invalid input data");

            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userCreationDto.Email || u.PhoneNumber == userCreationDto.PhoneNumber);
                if (existingUser != null)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("User with the provided email or phone number already exists.");
                }

                var hashedPassword = _passwordService.HashPassword(userCreationDto.Password);


                var user = new User
                {
                    Name = userCreationDto.Name,
                    Email = userCreationDto.Email,
                    Password = hashedPassword,
                    PhoneNumber = userCreationDto.PhoneNumber,
                    IsEmailVerified = false,
                    IsPhonenumberVerified = false,
                    Verificationstatus = "Pending"
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                var defaultRole = RoleType.Guest;
                var userRole = new UserRoles
                {
                    UserId = user.Id,
                    Role = defaultRole.ToString()
                };
                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok("User account is created successfully. Please verify your email " +
                    " complete registration successfully.");
            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }


        [HttpPost("verify-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Verify Email using OTP",
                          Description = "Verifies a user's email using an OTP. OTPType is optional and defaults to EmailVerification.")]
        public async Task<IActionResult> VerifyEmail([FromBody] OTPVerificationDTO otpVerificationDto)
        {

            if (otpVerificationDto == null)
            {
                return BadRequest("OTP verification data is not valid.");
            }

            var otpType = OTPType.EmailVerfication;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == otpVerificationDto.email);
            if (user == null)
            {
                return BadRequest("Invalid user.");
            }


            var otp = await _context.OTP
                .FirstOrDefaultAsync(o => o.UserId == user.Id
                                           && o.Code == otpVerificationDto.Code
                                           && o.OtpType == otpType
                                           && !o.IsUsed
                                           && o.ExpiryDate > DateTime.UtcNow);

            if (otp == null)
            {
                return BadRequest("Invalid or expired OTP code.");
            }

            otp.IsUsed = true;
            _context.OTP.Update(otp);
            await _context.SaveChangesAsync();

            user.IsEmailVerified = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            if (user.IsEmailVerified)
            {
                return Ok("User registration completed successfully.");
            }

            return Ok("Email verified successfully. registration is completed successfully");
        }


        [HttpPost("resend-email-otp")]

        public async Task<IActionResult> ResendEmailOtp([FromBody] ResendOtpDTO resendOtpDto)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == resendOtpDto.email);
            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            if (user.IsEmailVerified)
            {
                return BadRequest("Email is already verified.");
            }

            var newOtpCode = await _otpService.GenerateOTP();
            await _otpService.SaveOtpAsync(user.Id, newOtpCode, OTPType.EmailVerfication);

            await _emailSender.SendOtpEmailAsync(user.Email, newOtpCode);

            return Ok("New OTP sent to your email.");
        }



        [HttpPost("send-email-otp")]

        public async Task<IActionResult> SendEmailOtp([FromBody] EmailVerificationdto emailVerificationRequestDto)
        {
            if (string.IsNullOrEmpty(emailVerificationRequestDto.Email))
            {
                return BadRequest("Email address is required.");
            }


            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailVerificationRequestDto.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (user.IsEmailVerified)
            {
                return BadRequest("Email is already verified.");
            }


            var emailOtpCode = await _otpService.GenerateOTP();

            await _otpService.SaveOtpAsync(user.Id, emailOtpCode, OTPType.EmailVerfication);


            await _emailSender.SendOtpEmailAsync(user.Email, emailOtpCode);

            return Ok("OTP sent to email. Please verify your email.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDto)
        {
            try
            {
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                {
                    return BadRequest("Email and password are required.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (user == null || !_passwordService.VerifyPassword(loginDto.Password, user.Password))
                {
                    return Unauthorized("Invalid email or password.");
                }

                var token = _jwtService.GenerateJwtToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(60);
                _context.Users.Update(user);

                var refreshTokenEntity = new Refreshtoken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(60),
                    IsRevoked = false
                };
                _context.RefreshTokens.Add(refreshTokenEntity);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Token = token,
                    name = user.Name,
                    RefreshToken = refreshToken,
                    Message = "Login successful."
                });
            }
            catch (ArgumentNullException ex)
            {

                return BadRequest($"Invalid input: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {

                return StatusCode(500, $"Operation error: {ex.Message}");
            }
            catch (DbUpdateException ex)
            {

                return StatusCode(500, $"Database update error: {ex.Message}");
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshtokenDTO refreshTokenDto)
        {
            if (refreshTokenDto == null || string.IsNullOrEmpty(refreshTokenDto.Token))
            {
                return BadRequest("Refresh token data is not valid.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenDto.Token && u.RefreshTokenExpiryTime > DateTime.UtcNow);
            if (user == null)
            {
                return BadRequest("Invalid or expired refresh token.");
            }

            var newJwtToken = _jwtService.GenerateJwtToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();


            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(60); // Refresh token valid for 1 hour
            _context.Users.Update(user);


            var oldRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.Token);
            if (oldRefreshToken != null)
            {
                oldRefreshToken.IsRevoked = true;
                _context.RefreshTokens.Update(oldRefreshToken);
            }

            var refreshTokenEntity = new Refreshtoken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(60),
                IsRevoked = false
            };
            _context.RefreshTokens.Add(refreshTokenEntity);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpPost("forgot-password")]
        [SwaggerOperation(Summary = "Request a password reset link",
                           Description = "Generates a password reset token and sends a reset link to the user's email."
         )]
        [SwaggerResponse(StatusCodes.Status200OK, "Password reset link has been sent to your email.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "User with the provided email does not exist.")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);
            if (user == null)
            {
                return BadRequest("User with the provided email does not exist.");
            }

            var resetToken = Guid.NewGuid().ToString();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var resetUrl = $"http://localhost:4200/resetpassword?token={resetToken}";
            await _emailSender.SendPasswordResetLinkAsync(user.Email, resetToken);

            return Ok("Password reset link has been sent to your email.");
        }

        [HttpPost("reset-password")]
        [SwaggerOperation(
                         Summary = "Reset the user's password",
                      Description = "Resets the user's password using the provided token and new password."
         )]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordDTO resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token && u.PasswordResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                return BadRequest("Invalid or expired reset token.");
            }

            var hashedPassword = _passwordService.HashPassword(resetPasswordDto.NewPassword);
            user.Password = hashedPassword;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Password has been reset successfully.");
        }

   

    }
}