namespace CineMatrix_API.Models
{
    public class Subscribe
    {

        public int Id { get; set; } 
        public int UserId { get; set; } 
        public string PhoneNumber { get; set; }
        public bool IsVerified { get; set; }
        public bool IsPaymentSuccessful { get; set; }

        public User User { get; set; }

    }
}
