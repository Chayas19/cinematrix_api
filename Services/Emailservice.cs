
using CineMatrix_API.Repository;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CineMatrix_API;
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOtpEmailAsync(string toEmail, string otpCode)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var fromAddress = emailSettings["FromAddress"];
        var displayName = emailSettings["DisplayName"];
        var server = emailSettings["Server"];
        var port = int.Parse(emailSettings["Port"]);
        var userName = emailSettings["UserName"];
        var appPassword = emailSettings["AppPassword"];

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(displayName, fromAddress));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Your OTP Code";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"<p>Hi,</p>
                          <p>Thank you for registering. Your OTP is <strong>{otpCode}</strong>.</p>
                          <p>Please enter this OTP to verify your email address. The OTP is valid for 5 minutes.</p>
                          <p>If you did not register for an account, please ignore this email.</p>
                          <p>Thank you!</p>"
        };
        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(userName, appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
    }
    public async Task SendPasswordResetLinkAsync(string email, string resetToken)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Support", emailSettings["FromAddress"]));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = "Password Reset Request";

        var resetUrl = $"http://localhost:4200/resetpassword?token={resetToken}"; 

        message.Body = new TextPart("html")
        {
            Text = $@"
                <p>Hi there,</p>
                <p>You requested to reset your password. Please click the link below to reset it:</p>
                <p><a href='{resetUrl}'>Reset Password</a></p>
                <p>If you did not request this, please ignore this email.</p>
                <p>Thank you!</p>"
        };

        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings["Server"], int.Parse(emailSettings["Port"]), false);
                await client.AuthenticateAsync(emailSettings["UserName"], emailSettings["AppPassword"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
         
            throw new InvalidOperationException("Failed to send email.", ex);
        }




    }
    public async Task SendSubscriptionConfirmationEmailAsync(string toEmail, string subscriptionType, DateTime endDate)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var fromAddress = emailSettings["FromAddress"];
        var displayName = emailSettings["DisplayName"];
        var server = emailSettings["Server"];
        var port = int.Parse(emailSettings["Port"]);
        var userName = emailSettings["UserName"];
        var appPassword = emailSettings["AppPassword"];

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(displayName, fromAddress));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Subscription Confirmation";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"<p>Hi,</p>
                          <p>Thank you for subscribing to CineMatrix.</p>
                          <p>Your subscription type is <strong>{subscriptionType}</strong>.</p>
                          <p>Your subscription is valid until <strong>{endDate.ToShortDateString()}</strong>.</p>
                          <p>Thank you!</p>"
        };
        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(userName, appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
    }

    public async Task SendSubscriptionExpiryNotificationAsync(string toEmail, DateTime endDate)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var fromAddress = emailSettings["FromAddress"];
        var displayName = emailSettings["DisplayName"];
        var server = emailSettings["Server"];
        var port = int.Parse(emailSettings["Port"]);
        var userName = emailSettings["UserName"];
        var appPassword = emailSettings["AppPassword"];

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(displayName, fromAddress));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Subscription Expiry Notice";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"<p>Hi,</p>
                          <p>Your subscription is about to expire on <strong>{endDate.ToShortDateString()}</strong>.</p>
                          <p>Click <a href='http://localhost:4200/renewsubscription'>here</a> to renew your subscription and get an additional 3-day trial.</p>
                          <p>Thank you!</p>"
        };
        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(userName, appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
    }
}


