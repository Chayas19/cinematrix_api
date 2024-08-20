namespace CineMatrix_API.DTOs
{
    public class ReviewDTO
    {
        public int MovieId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
