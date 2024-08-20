namespace CineMatrix_API.Repository
{
    public interface ISMSService
    {
        Task SendOtpSmsAsync(string phoneNumber, string otpCode);
    }
}

