namespace CineMatrix_API.DTOs
{
    public class ResetPasswordDTO
    {

        public string ResetToken { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

}

