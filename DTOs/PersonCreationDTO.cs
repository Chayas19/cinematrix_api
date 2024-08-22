using System.ComponentModel.DataAnnotations;

namespace CineMatrix_API.DTOs
{
    public class PersonCreationDTO : PersonPatchDTO
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Biography { get; set; }

        [Required]
        public IFormFile Picture { get; set; }

        [Required]
        public string DateOfBirth { get; set; }
    }
}
