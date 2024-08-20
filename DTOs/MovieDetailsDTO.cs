namespace CineMatrix_API.DTOs
{
    public class MovieDetailsDTO : MovieDTO
    {

        public List<GenreDTO> Genres { get; set; }
        public List<ReviewDTO> Reviews { get; set; }
        public List<WatchHistoryDTO> WatchHistories { get; set; }
        public List<ActorDTO> Actors { get; set; }



    }
}

