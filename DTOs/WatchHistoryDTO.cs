namespace CineMatrix_API.DTOs
{
    public class WatchHistoryDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }
        public DateTime WatchedAt { get; set; }
        public TimeSpan WatchedDuration { get; set; } 
        public string MovieTitle { get; set; }
        public string UserName { get; set; } 
    }
}
