using System.ComponentModel.DataAnnotations;
using CineMatrix_API.Helpers;
using CinematrixAPI.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CineMatrix_API.DTOs
{
    public class MovieCreationDTO : MoviePatchDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }

        [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Duration must be in hh:mm format.")]
        public string Duration { get; set; }
        public string Director { get; set; }

        public string Language { get; set; }
        public bool IsFree { get; set; }
        public string PosterUrl { get; set; }
        public SubscriptionType SubscriptionType { get; set; }


        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenresIds { get; set; }

        [Required]
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorCreationDTO>>))]
        public List<ActorCreationDTO> Actors { get; set; }
    }
}


