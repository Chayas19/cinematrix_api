using System.ComponentModel.DataAnnotations;

namespace CineMatrix_API.DTOs
{
    public class PersonDTO
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Biography { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        public string Picture { get; set; }

        public string ImageUrl { get; set; }
    }
}
