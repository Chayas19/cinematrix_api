namespace CineMatrix_API.Models
{
    public class Refreshtoken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsRevoked { get; set; }
        public User User { get; set; }
    }
}
