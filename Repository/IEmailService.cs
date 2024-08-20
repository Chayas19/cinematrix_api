
namespace CineMatrix_API.Repository
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email, string otpCode);
        Task SendPasswordResetLinkAsync(string email, string resetToken);
        Task SendSubscriptionConfirmationEmailAsync(string toEmail, string subscriptionType, DateTime endDate);
        Task SendSubscriptionExpiryNotificationAsync(string toEmail, DateTime endDate);
    }
}

