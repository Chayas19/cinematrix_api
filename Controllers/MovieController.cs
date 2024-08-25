using AutoMapper;
using CineMatrix_API.DTOs;
using CineMatrix_API.Helpers;
using CineMatrix_API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMatrix_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public MovieController(ApplicationDbContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }

        // POST: api/movie
        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] MovieCreationDTO movieCreationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data provided.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
               

                byte[] posterData = null;
                if (!string.IsNullOrWhiteSpace(movieCreationDTO.PosterUrl))
                {
                    string imageDirectory = Path.Combine(_env.WebRootPath, "images");
                    string sourceFilePath = Path.Combine(imageDirectory, movieCreationDTO.PosterUrl);

                    if (System.IO.File.Exists(sourceFilePath))
                    {
                        posterData = await System.IO.File.ReadAllBytesAsync(sourceFilePath);
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "The specified poster file does not exist." });
                    }
                }
                var movieExists = await _context.Movies
           .AnyAsync(m => EF.Functions.Like(m.Title, movieCreationDTO.Title));

                if (movieExists)
                {
                    return BadRequest(new { success = false, message = "A movie with the same title already exists." });
                }


                var movie = new Movie
                {
                    Title = movieCreationDTO.Title,
                    Description = movieCreationDTO.Description,
                    Duration = TimeSpan.ParseExact(movieCreationDTO.Duration, "hh\\:mm", null),
                    Language = movieCreationDTO.Language,
                    IsFree = movieCreationDTO.IsFree,
                    Director = movieCreationDTO.Director,
                    PosterUrl = movieCreationDTO.PosterUrl,
                    PosterData = posterData,
                    SubscriptionType = movieCreationDTO.SubscriptionType,
                };

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(movieCreationDTO.Language))
                {
                    var language = await _context.Languages
                        .Where(l => l.Name.ToLower() == movieCreationDTO.Language.ToLower())
                        .FirstOrDefaultAsync();

                    if (language == null)
                    {
                        return BadRequest(new { success = false, message = "The specified language does not exist." });
                    }

                    var movieLanguage = new MovieLanguage
                    {
                        MovieId = movie.Id,
                        LanguageId = language.Id
                    };

                    _context.MoviesLanguages.Add(movieLanguage);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest(new { success = false, message = "Language name is required." });
                }



                if (movieCreationDTO.Actors != null && movieCreationDTO.Actors.Any())
                {
                    foreach (var actorDTO in movieCreationDTO.Actors)
                    {
                        if (!await _context.Actors.AnyAsync(a => a.Id == actorDTO.PersonId))
                        {
                            return BadRequest(new { success = false, message = $"Actor with ID {actorDTO.PersonId} does not exist." });
                        }

                        _context.MovieActors.Add(new MovieActors
                        {
                            MovieId = movie.Id,
                            ActorId = actorDTO.PersonId,
                            Character = actorDTO.Character
                        });
                    }
                }

                if (movieCreationDTO.GenresIds != null && movieCreationDTO.GenresIds.Any())
                {
                    foreach (var genreId in movieCreationDTO.GenresIds)
                    {
                        if (!await _context.Genres.AnyAsync(g => g.Id == genreId))
                        {
                            return BadRequest(new { success = false, message = $"Genre with ID {genreId} does not exist." });
                        }

                        _context.MovieGenres.Add(new MovieGenres
                        {
                            MovieId = movie.Id,
                            GenreId = genreId
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = movie.Id }, new { success = true, message = "Movie created successfully." });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while creating the movie. Please try again later." });
            }
        }


        // GET: api/movie
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var queryable = _context.Movies.AsQueryable();


                await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);


                var movies = await queryable.Paginate(pagination).ToListAsync();

                if (movies == null || !movies.Any())
                {
                    return NotFound(new { success = false, message = "No movies found." });
                }

               
                var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

                var baseUrl = $"{Request.Scheme}://{Request.Host}/images/";

                foreach (var movieDTO in movieDTOs)
                {
                    if (movieDTO.PosterData != null && movieDTO.PosterData.Length > 0)
                    {
                        try
                        {
                           
                            var base64String = Convert.ToBase64String(movieDTO.PosterData);

                       
                            movieDTO.ImageUrl = $"data:image/jpeg;base64,{base64String}";
                        }
                        catch (Exception ex)
                        {
                            
                            
                            movieDTO.ImageUrl = $"{baseUrl}default.jpg";
                        }
                    }
                    else if (!string.IsNullOrEmpty(movieDTO.Poster))
                    {
                        movieDTO.ImageUrl = $"{baseUrl}{movieDTO.Poster}";
                    }
                    else
                    {
                        movieDTO.ImageUrl = $"{baseUrl}default.jpg"; // Ensure this image exists
                    }
                }

                return Ok(new { success = true, message = "Movies retrieved successfully.", data = movieDTOs });
            }
            catch (Exception ex)
            {
             
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while fetching movies. Please try again later." });
            }
        }



        [HttpGet("{id}")]
        // GET: api/movie/{id}
        public async Task<IActionResult> GetMovieImage(int id)
        {
            try
            {
               
                var movie = await _context.Movies.FindAsync(id);

                if (movie == null)
                {
                    return NotFound(new { success = false, message = "Movie not found." });
                }

             
                var movieDetails = new
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    Duration = movie.Duration,
                    Language = movie.Language,
                    IsFree = movie.IsFree,
                    Director = movie.Director,
                    PosterUrl = movie.PosterUrl,
                    SubscriptionType = movie.SubscriptionType,
                    ImageUrl = movie.PosterData != null && movie.PosterData.Length > 0
                        ? $"data:image/jpeg;base64,{Convert.ToBase64String(movie.PosterData)}"
                        : null
                };

                return Ok(new { success = true, data = movieDetails });
            }
            catch (Exception ex)
            {
                

                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while fetching the movie details. Please try again later." });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(id);

                if (movie == null)
                {
                    return NotFound(new { success = false, message = "Movie not found." });
                }


                var movieActors = _context.MovieActors.Where(ma => ma.MovieId == id);
                _context.MovieActors.RemoveRange(movieActors);


                var movieGenres = _context.MovieGenres.Where(mg => mg.MovieId == id);
                _context.MovieGenres.RemoveRange(movieGenres);

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Movie deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while deleting the movie. Please try again later." });
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieCreationDTO movieCreationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data provided.");
            }

            var movie = await _context.Movies
                .Include(m => m.MoviesActors)
                .Include(m => m.MoviesGenres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound("Movie not found.");
            }

            movie.Title = movieCreationDTO.Title ?? movie.Title;
            movie.Description = movieCreationDTO.Description ?? movie.Description;
            movie.Duration = !string.IsNullOrWhiteSpace(movieCreationDTO.Duration)
                ? TimeSpan.ParseExact(movieCreationDTO.Duration, "hh\\:mm", null)
                : movie.Duration;
            movie.Language = movieCreationDTO.Language ?? movie.Language;
            movie.IsFree = movieCreationDTO.IsFree;
            movie.Director = movieCreationDTO.Director ?? movie.Director;

            movie.SubscriptionType = movieCreationDTO.SubscriptionType;


            if (!string.IsNullOrWhiteSpace(movieCreationDTO.PosterUrl))
            {
                string sourceFilePath = Path.Combine(_env.WebRootPath, "images", movieCreationDTO.PosterUrl);
                if (System.IO.File.Exists(sourceFilePath))
                {
                    movie.PosterData = await System.IO.File.ReadAllBytesAsync(sourceFilePath);
                }
            }
            else
            {
                movie.PosterData = null;
            }



            if (movieCreationDTO.Actors != null)
            {
                _context.MovieActors.RemoveRange(movie.MoviesActors);
                foreach (var actorDTO in movieCreationDTO.Actors)
                {
                    if (await _context.Actors.AnyAsync(a => a.Id == actorDTO.PersonId))
                    {
                        _context.MovieActors.Add(new MovieActors
                        {
                            MovieId = movie.Id,
                            ActorId = actorDTO.PersonId,
                            Character = actorDTO.Character ?? movie.MoviesActors.FirstOrDefault(ma => ma.ActorId == actorDTO.PersonId)?.Character
                        });
                    }
                }
            }

            if (movieCreationDTO.GenresIds != null)
            {
                _context.MovieGenres.RemoveRange(movie.MoviesGenres);
                foreach (var genreId in movieCreationDTO.GenresIds)
                {
                    if (await _context.Genres.AnyAsync(g => g.Id == genreId))
                    {
                        _context.MovieGenres.Add(new MovieGenres
                        {
                            MovieId = movie.Id,
                            GenreId = genreId
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Movie updated successfully.");
        }




    }


}
