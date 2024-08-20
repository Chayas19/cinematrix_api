namespace CineMatrix_API.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateOnly Dateofbirth { get; set; }
        public string PictureUrl { get; set; } 

        public byte[] Picture { get; set; }
        public List<MovieActors> MoviesActors { get; set; }


    }
}
