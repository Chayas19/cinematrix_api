using CineMatrix_API.Enums;

namespace CineMatrix_API.Repository
{
    public interface IOtpService
    {
        Task<string> GenerateOTP();
        Task SaveOtpAsync(int userId, string otpCode, OTPType otpType);
        Task<bool> ValidateOtpAsync(int userId, string otpCode, OTPType otpType);
    }
}