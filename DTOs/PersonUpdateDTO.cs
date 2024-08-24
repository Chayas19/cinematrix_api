using System.ComponentModel.DataAnnotations;

namespace CineMatrix_API.DTOs
{
    public class PersonUpdateDTO
    {


        public string Name { get; set; }

        public string Biography { get; set; }


        public IFormFile Picture { get; set; }


        public string DateOfBirth { get; set; }
    }
}
