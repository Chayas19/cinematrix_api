using CineMatrix_API.Enums;

namespace CineMatrix_API.Models
{
    public class OTPVerification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public OTPType OtpType { get; set; }
    }
}
