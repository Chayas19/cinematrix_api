using System.Linq.Expressions;
using AutoMapper;
using CineMatrix_API;
using CineMatrix_API.Controllers;
using CineMatrix_API.DTOs;
using CineMatrix_API.Enums;
using CineMatrix_API.Models;
using CineMatrix_API.Repository;
using CineMatrix_API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using SQLitePCL;
using Twilio.Rest.Serverless.V1.Service;
using Vonage.Common.Monads;
using Xunit;


public class UserAccountControllerTests
{
    private readonly UserAccountController _controller;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly Mock<IOtpService> _mockOtpService;
    private readonly Mock<ISMSService> _mockSmsService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<Ijwtservice> _mockJwtService;
    private readonly Mock<IValidator<UsercreationDTO>> _mockValidator;
    private readonly Mock<IValidator<EmailVerificationdto>> _mockemailValidator;
    private readonly Mock<IValidator<ResendOtpDTO>> _mockResendOtpValidator;  

    public UserAccountControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new ApplicationDbContext(options);
        _mapper = new Mock<IMapper>().Object;
        _mockOtpService = new Mock<IOtpService>();
        _mockSmsService = new Mock<ISMSService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockJwtService = new Mock<Ijwtservice>();
        _mockValidator = new Mock<IValidator<UsercreationDTO>>();
        _mockemailValidator = new Mock<IValidator<EmailVerificationdto>>();
        _mockResendOtpValidator = new Mock<IValidator<ResendOtpDTO>>(); 

        _controller = new UserAccountController(
            _mapper,
            _context,
             _mockOtpService.Object,
            _mockSmsService.Object,
            _mockEmailService.Object,
            _mockPasswordService.Object,
            _mockJwtService.Object,
            _mockValidator.Object,
            _mockemailValidator.Object,
            _mockResendOtpValidator.Object
           
        );
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        var userdto = new UsercreationDTO
        {
            Name = "JohnDoe",
            Email = "JohnDoe@example.com",
            Password = "JohnDoe@123",
            ConfirmPassword = "JohnDoe@123",
            PhoneNumber = 1234567890
        };

        _mockValidator.Setup(v => v.Validate(It.IsAny<UsercreationDTO>()))
                      .Returns(new FluentValidation.Results.ValidationResult());

        _mockPasswordService.Setup(s => s.HashPassword(It.IsAny<string>()))
                            .Returns("hashedPassword");

        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();

        var result = await _controller.Register(userdto);
        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, actionResult.StatusCode);
        Assert.Equal("User account is created successfully. Please verify your email to complete registration", actionResult.Value);


    }


    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        var userdto = new UsercreationDTO
        {
            Name = "JohnDoe",
            Email = "existing@example.com",
            Password = "JohnDoe@123",
            ConfirmPassword = "JohnDoe@123",
            PhoneNumber = 1234567890
        };

        var existingUser = new User
        {
            Name = "Existing User",
            Email = "existing@example.com",
            Password = "existingPassword",
            PhoneNumber = 1234567890,
            IsEmailVerified = true,
            Verificationstatus = "false",
            IsPhonenumberVerified = true,

        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();


        _mockValidator.Setup(v => v.Validate(It.IsAny<UsercreationDTO>()))
                      .Returns(new FluentValidation.Results.ValidationResult());

        var result = await _controller.Register(userdto);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
        Assert.Equal("User with the provided email or phone number already exists.", actionResult.Value);
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        var userdto = new UsercreationDTO
        {
            Name = "JohnDoe",
            Email = "string",
            Password = "JohnDoe@123",
            ConfirmPassword = "JohnDoe@123",
            PhoneNumber = 1234567890
        };

        _mockValidator.Setup(v => v.Validate(It.IsAny<UsercreationDTO>()))
                      .Returns(new FluentValidation.Results.ValidationResult());

        var result = await _controller.Register(userdto);

        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
        Assert.Equal("Invalid input data", actionResult.Value);
    }

    [Fact]
    public async Task Register_WithPasswordMismatch_ReturnsBadRequest()
    {
        var userdto = new UsercreationDTO
        {
            Name = "JohnDoe",
            Email = "JohnDoe@example.com",
            Password = "JohnDoe@123",
            ConfirmPassword = "Different@123",
            PhoneNumber = 1234567890

        };
        _mockValidator.Setup(v => v.Validate(It.IsAny<UsercreationDTO>()))
            .Returns(new FluentValidation.Results.ValidationResult());
        var result = await _controller.Register(userdto);
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
        Assert.Equal("Password fields cannot be empty and must match.", actionResult.Value);
    }

    [Fact]
    public async Task ReturnsEmptyFieldsData_ReturnsBadRequest()
    {
        var userdto = new UsercreationDTO
        {
            Name = "",
            Email = "",
            Password = "",
            ConfirmPassword = "",
            PhoneNumber = -1
        };

        _mockValidator.Setup(v => v.Validate(It.IsAny<UsercreationDTO>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        var result = await _controller.Register(userdto);
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
        Assert.Equal("Fields cannot be empty, Please provide the valid input data", actionResult.Value);

    }

    [Fact]
    public async Task Returns_stringInputDataFormat_ReturnsBadRequest()
    {
        var userdto = new UsercreationDTO
        {
            Name = "string",
            Email = "string",
            Password = "string",
            ConfirmPassword = "string",
            PhoneNumber = 0
        };

        _mockValidator.Setup(v => v.Validate(It.IsAny<UsercreationDTO>()))
                      .Returns(new FluentValidation.Results.ValidationResult());

        var result = await _controller.Register(userdto);
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
        Assert.Equal("Fields cannot be empty, Please provide the valid input data", actionResult.Value);

    }

    [Fact]

    public async Task Register_duplicatePhoneNumber_ReturnsBadRequest()
    {
        var userdto = new UsercreationDTO()
        {
            Name = "JohnDoe",
            Email = "JohnDoe@example.com",
            PhoneNumber = 1234567890,
            Password = "JohnDoe@123",
            ConfirmPassword = "JohnDoe@123",
        };
        var existingUser = new User
        {
            Name = "Existing User",
            Email = "existing@example.com",
            Password = "existingPassword",
            PhoneNumber = 1234567890,
            IsEmailVerified = true,
            Verificationstatus = "false",
            IsPhonenumberVerified = true,

        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();


        _mockValidator.Setup(v => v.Validate(It.IsAny<UsercreationDTO>()))
             .Returns(new FluentValidation.Results.ValidationResult());

        var result = await _controller.Register(userdto);
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, actionResult.StatusCode);

    }
 

    [Fact]

    public async Task SendEmailOtp_InvalidEmail_ReturnsBadRequest()
    {

        var emaildto = new EmailVerificationdto
        {
            Email = "JohnDoe@^&^&*)_+++==="
        };

        _mockemailValidator.Setup(v => v.Validate(It.IsAny<EmailVerificationdto>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        var result = await _controller.SendEmailOtp(emaildto);
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, actionResult.StatusCode);

    }

    [Fact]

    public async Task SendEmailOtp_ValidEmail_ReturnSuccess()
    {
        var email = new EmailVerificationdto
        {
            Email = "JohnDoe@example.com"
        };


        _mockOtpService.Setup(s => s.GenerateOTP())
            .ReturnsAsync("12345");

        _mockOtpService.Setup(s => s.SaveOtpAsync(It.IsAny<int>(), "12345",OTPType.EmailVerfication))
            .Returns(Task.CompletedTask);

        _mockEmailService.Setup(s => s.SendOtpEmailAsync(It.IsAny<string>(), "12345"))
       .Returns(Task.CompletedTask);

        var existingUser = new User
        {
            Name = "Existing User",
            Email = "JohnDoe@example.com",
            Password = "existingPassword",
            PhoneNumber = 1234567890,
            IsEmailVerified = false,
            Verificationstatus = "false",
            IsPhonenumberVerified = true,

        };

        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        _mockemailValidator.Setup(v => v.Validate(It.IsAny<EmailVerificationdto>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        var result = await _controller.SendEmailOtp(email);
        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, actionResult.StatusCode);
        Assert.Equal("OTP sent to email. Please verify your email.", actionResult.Value);
    }

    [Fact]
    public async Task SendEmailOtp_UserNotExists_ReturnsBadRequest()
    {
        var emaildto = new EmailVerificationdto
        {
            Email = "JohnDoe12@example.com"
        };

        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();

        _mockemailValidator.Setup(v => v.Validate(It.IsAny<EmailVerificationdto>()))
            .Returns(new FluentValidation.Results.ValidationResult());
        var result = await _controller.SendEmailOtp(emaildto);
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
        Assert.Equal("User does not exists", actionResult.Value);
    }

    [Fact]
    public async Task SendEmailOtp_InternalServerError_ReturnsBadRequest()
    {
        var email = new EmailVerificationdto
        {
            Email = "JohnDoe@example.com"
        };
        _mockemailValidator.Setup(v => v.Validate(It.IsAny<EmailVerificationdto>()))
            .Returns(new FluentValidation.Results.ValidationResult());


        _mockEmailService.Setup(s => s.SendOtpEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
     .ThrowsAsync(new Exception("Internal server error"));


        var result = await _controller.SendEmailOtp(email);
        var actionResult = Assert.IsType<ObjectResult>(result);        Assert.Equal(500, actionResult.StatusCode);
        Assert.Equal("Internal server error", actionResult.Value);

    }

    [Fact]

    public async Task ResendEmailOtp_ReturnsValidData()
    {
        var resend = new ResendOtpDTO
        {
            UserId = 1,
            email = "JohnDoe@example.com"
        };

        _mockResendOtpValidator.Setup(v => v.Validate(It.IsAny<ResendOtpDTO>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        _mockOtpService.Setup(v => v.GenerateOTP())
            .ReturnsAsync("85272");

        _mockOtpService.Setup(s => s.SaveOtpAsync(It.IsAny<int>(), "85272", OTPType.EmailVerfication))
            .Returns(Task.CompletedTask);

        _mockEmailService.Setup(s => s.SendOtpEmailAsync(It.IsAny<string>(), "85272"))

            .Returns(Task.CompletedTask);

        var user = new User
        {
            Id = 1,
            Name = "JohnDoe",
            Verificationstatus = "false",
            Email = "JohnDoe@example.com",
            IsEmailVerified = false,
            PhoneNumber = 1234567890,
            Password = "hashedPassword",

        };

        _context.Users.Add(user);   
        await _context.SaveChangesAsync();  
      
        var result = await _controller.ResendEmailOtp(resend);  
        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, actionResult.StatusCode);
        Assert.Equal("New OTP sent to your email.", actionResult.Value);

    }

   
}




