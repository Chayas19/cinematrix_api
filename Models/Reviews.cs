using System.ComponentModel.DataAnnotations;

namespace CineMatrix_API.Models
{
    public class Reviews
    {
        [Key]
        public int Id { get; set; }
        public int MovieId { get; set; }
         public Movie Movie { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
