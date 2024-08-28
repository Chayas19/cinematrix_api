using CineMatrix_API.Enums;
using CineMatrix_API.Models;
using CineMatrix_API.Repository;
using Microsoft.EntityFrameworkCore;

namespace CineMatrix_API.Services
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _context;

        public OtpService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateOTP()
        {
            var otpCode = new Random().Next(10000, 99999).ToString();
            return await Task.FromResult(otpCode);
        }

        public async Task SaveOtpAsync(int userId, string otpCode, OTPType otpType)
        {
            var otp = new OTPVerification
            {
                UserId = userId,
                Code = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                OtpType = otpType
            };

            _context.OTP.Add(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateOtpAsync(int userId, string otpCode, OTPType otpType)
        {
            var otp = await _context.OTP
                .FirstOrDefaultAsync(o => o.UserId == userId
                                           && o.Code == otpCode
                                           && o.OtpType == otpType
                                           && !o.IsUsed
                                           && o.ExpiryDate > DateTime.UtcNow);

            if (otp != null)
            {
                otp.IsUsed = true;
                _context.OTP.Update(otp);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
