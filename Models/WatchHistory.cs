namespace CineMatrix_API.Models
{
    public class WatchHistory

    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public DateTime WatchedAt { get; set; }
        public TimeSpan WatchedDuration { get; set; }
    }
}

