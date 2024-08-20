namespace CineMatrix_API.Models
{
    public class ResetPassword
    {
        public string ResetToken { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
