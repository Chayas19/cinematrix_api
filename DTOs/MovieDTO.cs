using CinematrixAPI.Enums;

namespace CineMatrix_API.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; } 
        public string Language { get; set; }
        public bool IsFree { get; set; }

        public byte[] PosterData { get; set; }
        public string Poster { get; set; }

        public string ImageUrl { get; set; } 

        public SubscriptionType SubscriptionType { get; set; }
    }
}
