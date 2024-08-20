using System.ComponentModel.DataAnnotations;

namespace CineMatrix_API.DTOs
{
    public class PersonPatchDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public string Biography { get; set; }
        public string DateOfBirth { get; set; }
    }
}
