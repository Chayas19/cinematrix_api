using CineMatrix_API.Enums;

namespace CineMatrix_API.DTOs
{
    public class OTPVerificationDTO
    {
        // public int UserId { get; set; }

        public string email { get; set; }
        public string Code { get; set; }
        public OTPType OtpType { get; set; }
    }
}
