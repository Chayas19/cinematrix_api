using System.ComponentModel.DataAnnotations;
using CinematrixAPI.Enums;

namespace CineMatrix_API.Models
{
    public class Movie
    {

        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Duration must be in hh:mm format.")]
        public TimeSpan Duration { get; set; } 
        public string Language { get; set; }
        public bool IsFree { get; set; }
        public string Director { get; set; }

        public byte[] PosterData { get; set; }

        public string PosterUrl { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public List<MovieActors> MoviesActors { get; set; }
        public List<MovieGenres> MoviesGenres { get; set; }
        public List<MovieLanguage> MovieLanguages { get; set; }
        public ICollection<Reviews> Reviews { get; set; }
        public ICollection<WatchHistory> WatchHistories { get; set; }
     


    }

}
