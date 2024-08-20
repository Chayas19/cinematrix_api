using CineMatrix_API.Repository;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;




namespace CineMatrix_API.Services
{
   

    public class Smsservice : ISMSService
    {

        private readonly IConfiguration _configuration;

        public Smsservice(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task SendOtpSmsAsync(string phoneNumber, string otpCode)
        {
          
            phoneNumber = FormatPhoneNumber(phoneNumber);

            var twilioSettings = _configuration.GetSection("TwilioSettings");
            var accountSid = twilioSettings["AccountSid"];
            var authToken = twilioSettings["AuthToken"];
            var fromNumber = twilioSettings["FromNumber"];

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var message = await MessageResource.CreateAsync(
                    body: $"Your OTP code for CineMatrix is: {otpCode}. Please use this code to verify your phone number.",
                    from: new PhoneNumber(fromNumber),
                    to: new PhoneNumber(phoneNumber)
                );

                Console.WriteLine($"Message sent with SID: {message.Sid}");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                throw;
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
           
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

        
            if (!digitsOnly.StartsWith("91"))
            {
                digitsOnly = "91" + digitsOnly;
            }

            
            return $"+{digitsOnly}";
        }


    }
}

