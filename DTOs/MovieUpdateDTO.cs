using System.ComponentModel.DataAnnotations;

namespace CineMatrix_API.DTOs
{
    public class MovieUpdateDTO
    {

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Invalid duration format. Use 'hh:mm'.")]
        public string Duration { get; set; }
        public string Language { get; set; }

        public bool? IsFree { get; set; }

        public string Director { get; set; }

        public string PosterUrl { get; set; }

        public List<int> GenresIds { get; set; }

        public List<ActorDTO> Actors { get; set; }

        public SubscriptionType? subscriptionType { get; set; }

        public class ActorDTO
        {
            [Required]
            public int PersonId { get; set; }

            public string Character { get; set; }
        }

        public enum SubscriptionType
        {
            Free,
            Premiumn
        }
    }

}

