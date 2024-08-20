namespace CineMatrix_API.Models
{
    public class MovieGenres
    {

        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public Movie Movie { get; set; }
        public Genre Genre { get; set; }
    }
}

