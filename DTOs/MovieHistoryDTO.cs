namespace CineMatrix_API.DTOs
{
    public class MovieHistoryDTO
    {

        public int MovieId { get; set; }
        public int UserId { get; set; }
        public DateTime WatchedAt { get; set; }
        public TimeSpan WatchedDuration { get; set; }
    }
}
