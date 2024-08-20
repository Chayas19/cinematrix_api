namespace CineMatrix_API.Services
{
    public class Passwordservice
    {

      
        public string HashPassword(string password)
        {
         
            string hash = BCrypt.Net.BCrypt.HashPassword(password);

         
            Console.WriteLine($"Hashed Password: {hash}");

            return hash;
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, storedHash);

         
            Console.WriteLine($"Password Verification: {isPasswordCorrect}");

            return isPasswordCorrect;
        }

    }
}
