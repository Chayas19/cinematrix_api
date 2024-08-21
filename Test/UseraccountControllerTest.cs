//using AutoMapper;
//using CineMatrix_API.Controllers;
//using CineMatrix_API.DTOs;
//using CineMatrix_API.Models;
//using CineMatrix_API.Repository;
//using CineMatrix_API.Services;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Xunit;

//namespace CineMatrix_API.Tests
//{
//    public class UserAccountControllerTests
//    {
//        private readonly UserAccountController _controller;
//        private readonly Mock<ApplicationDbContext> _mockDbContext;
//        private readonly Mock<IMapper> _mockMapper;
//        private readonly Mock<IEmailService> _mockEmailService;
//        private readonly Mock<ISMSService> _mockSmsService;
//        private readonly Mock<OtpService> _mockOtpService;
//        private readonly Mock<JwtService> _mockJwtService;
//        private readonly Mock<Passwordservice> _mockPasswordService;

//        public UserAccountControllerTests()
//        {
//            _mockDbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
//            _mockMapper = new Mock<IMapper>();
//            _mockEmailService = new Mock<IEmailService>();
//            _mockSmsService = new Mock<ISMSService>();
//            _mockOtpService = new Mock<OtpService>();
//            _mockJwtService = new Mock<JwtService>();
//            _mockPasswordService = new Mock<Passwordservice>();

//            _controller = new UserAccountController(
//                _mockMapper.Object,
//                _mockDbContext.Object,
//                _mockOtpService.Object,
//                _mockSmsService.Object,
//                _mockEmailService.Object,
//                _mockPasswordService.Object,
//                _mockJwtService.Object
//            );
//        }

//        [Fact]
//        public async Task Register_ReturnsBadReuest_WhenEmailExists()
//        {
//            var Usercreationdto = new UsercreationDTO
//            {
//                Name = "John Doe",
//                Email = "john.doe@example.com",
//                Password = "Password123",
//                ConfirmPassword = "Password123",
//                PhoneNumber = 1234567890
//            };
//            var user = new UsercreationDTO 
//            { 
//                Email = "john.doe@example.com" 
            
//            };
//            _mockDbContext.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>())).ReturnsAsync(user);


//        }
//    }
//}
