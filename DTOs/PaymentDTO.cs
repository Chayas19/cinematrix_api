using CinematrixAPI.Enums;

namespace CineMatrix_API.DTOs
{
    public class PaymentDTO
    {

        public int UserId { get; set; }
        public string OtpCode { get; set; }
        public decimal Amount { get; set; }
        public SubscriptionType SubscriptionType { get; set; }

    }
}
