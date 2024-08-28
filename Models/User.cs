namespace CineMatrix_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long PhoneNumber { get; set; } 
        public bool IsEmailVerified { get; set; }
        public bool IsPhonenumberVerified { get; set; }
       
        public string? PasswordResetToken { get; set; } 
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public string Verificationstatus { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public int? AccessCount { get; set; }
        public ICollection<UserRoles> Roles { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Reviews> Reviews { get; set; }
        public ICollection<OTPVerification> OtpCodes { get; set; }
        public ICollection<Refreshtoken> RefreshTokens { get; set; }
        public ICollection<WatchHistory> WatchHistories { get; set; }

        public ICollection<Subscribe> Subscribes { get; set; }

  
    }
}
