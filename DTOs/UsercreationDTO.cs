namespace CineMatrix_API.DTOs
{
    public class UsercreationDTO
    {

        public string Name { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public long PhoneNumber { get; set; }
        public string? ConfirmPassword { get; set; }
       

    }
}
