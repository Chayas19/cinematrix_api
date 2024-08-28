using System.Linq.Expressions;
using AutoMapper;
using CineMatrix_API;
using CineMatrix_API.Controllers;
using CineMatrix_API.DTOs;
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

        _controller = new UserAccountController(
            _mapper,
            _context,
             _mockOtpService.Object,
            _mockSmsService.Object,
            _mockEmailService.Object,
            _mockPasswordService.Object,
            _mockJwtService.Object,
            _mockValidator.Object
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

        var result = await _controller.Register(userdto);

        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, actionResult.StatusCode);
        Assert.Equal("User account is created successfully. Please verify your email  complete registration successfully.", actionResult.Value);
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
            Verificationstatus = "true",
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
    public async Task Register_WithPasswordmismatch_ReturnsBadRequest()
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
    public async Task ReturnsEmptyfieldsdata_ReturnsBadRequest()
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
    public async Task Returns_stringinputdataformat_ReturnsBadRequest()
    {
        var userdto = new UsercreationDTO()
        {
           Name = "string",
           Email = "string",
           Password = "string", 
           ConfirmPassword= "string",   
           PhoneNumber = 0

        };
        _mockValidator.Setup(v => v.Validate(It.IsAny<UsercreationDTO>()))
            .Returns(new FluentValidation.Results.ValidationResult());

        var result = await _controller.Register(userdto);
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, actionResult.StatusCode);
        Assert.Equal("Invalid input data", actionResult.Value);

    }
   
}




