
namespace CineMatrix_API.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<MovieGenres> MoviesGenres { get; set; }
    }

}
