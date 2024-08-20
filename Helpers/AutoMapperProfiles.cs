using AutoMapper;
using CineMatrix_API.DTOs;
using CineMatrix_API.Models;

namespace CineMatrix_API
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();

            CreateMap<User, UsercreationDTO>().ReverseMap();
            CreateMap<Payment, PaymentDTO>();
            CreateMap<Refreshtoken, RefreshtokenDTO>();
            CreateMap<Reviews, ReviewDTO>();
            CreateMap<Subscription, SubscriptionDTO>();
            CreateMap<UserRoles, UserRolesDTO>();

            CreateMap<ForgotPasswordDTO, User>();
            CreateMap<ResetPasswordDTO, User>();
            CreateMap<Movie, MovieDTO>()
         
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<Actor, PersonDTO>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Dateofbirth));
            CreateMap<Actor, PersonPatchDTO>().ReverseMap();

            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.PosterUrl, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieDetailsDTO>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MoviePatchDTO>().ReverseMap();

            CreateMap<Language, LanguageDTO>().ReverseMap();
            CreateMap<MovieLanguage, MovielanguageDTO>()
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
            CreateMap<MovieLanguageCreationDTO, MovieLanguage>();

            CreateMap<Movie, MovieDTO>();

            CreateMap<Actor, PersonDTO>()
           .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Dateofbirth.ToString("dd-MM-yyyy")));

            CreateMap<PersonCreationDTO, Actor>()
                .ForMember(dest => dest.Dateofbirth, opt => opt.MapFrom(src => DateOnly.Parse(src.DateOfBirth)));

            CreateMap<Movie, MovieDTO>()
             .ForMember(dest => dest.Poster, opt => opt.MapFrom(src => src.PosterUrl));

            CreateMap<MoviePoster, MovieDTO>()
          .ForMember(dest => dest.PosterData, opt => opt.MapFrom(src => src.PosterData));

            CreateMap<Movie, MovieDTO>()
.ForMember(dest => dest.PosterData, opt => opt.MapFrom(src => src.PosterData));
            //CreateMap<WatchHistory, WatchHistoryDTO>()
            //    .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
            //    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));

            //CreateMap<MovieHistoryDTO, WatchHistory>()
            //    .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.MovieId))
            //    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            //    .ForMember(dest => dest.WatchedAt, opt => opt.MapFrom(src => src.WatchedAt))
            //    .ForMember(dest => dest.WatchedDuration, opt => opt.MapFrom(src => src.WatchedDuration));

            CreateMap<UserRolesDTO, UserRoles>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            
            CreateMap<subscribecreationdto, Subscribe>()
                .ForMember(dest => dest.IsVerified, opt => opt.Ignore()) 
                .ForMember(dest => dest.IsPaymentSuccessful, opt => opt.Ignore()); 

         
            CreateMap<Subscribe, subscribecreationdto>();
        }

       

        private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<GenreDTO>();

            // Check if MoviesGenres is null
            if (movie.MoviesGenres != null)
            {
                foreach (var movieGenre in movie.MoviesGenres)
                {
                  
                    if (movieGenre != null && movieGenre.Genre != null)
                    {
                        result.Add(new GenreDTO
                        {
                            Id = movieGenre.GenreId,
                            Name = movieGenre.Genre.Name
                        });
                    }
                }
            }

            return result;
        }
        private List<ActorDTO> MapMoviesActors(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<ActorDTO>();

          
            if (movie.MoviesActors != null)
            {
                foreach (var actor in movie.MoviesActors)
                {
                  
                    if (actor != null && actor.Actor != null)
                    {
                        result.Add(new ActorDTO
                        {
                            Id = actor.ActorId,
                            Character = actor.Character,
                            Name = actor.Actor.Name
                        });
                    }
                }
            }

            return result;
        }
        private List<MovieGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MovieGenres>();

          
            if (movieCreationDTO.GenresIds != null)
            {
                foreach (var id in movieCreationDTO.GenresIds)
                {
                    result.Add(new MovieGenres { GenreId = id });
                }
            }

            return result;
        }

        private List<MovieActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MovieActors>();

       
            if (movieCreationDTO.Actors != null)
            {
                foreach (var actor in movieCreationDTO.Actors)
                {
                
                    if (actor != null)
                    {
                        result.Add(new MovieActors
                        {
                            ActorId = actor.PersonId,
                            Character = actor.Character
                        });
                    }
                }
            }

            return result;
        }
    }
}
