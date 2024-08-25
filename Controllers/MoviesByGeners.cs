using AutoMapper;
using CineMatrix_API.DTOs;
using CineMatrix_API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CineMatrix_API.Controllers
{
    [Route("api/movies/genres")]
    [ApiController]
    public class MoviesByGenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MoviesByGenresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet("filterByGenres")]
        [SwaggerOperation(
            Summary = "Get movies by genre IDs",
            Description = "Retrieves a list of movies filtered by the specified genre ID. The results are paginated according to the provided pagination parameters. Returns BadRequest if the genre ID is invalid."
        )]
        public async Task<ActionResult<List<MovieDTO>>> GetMoviesByGenres(
    [FromQuery] int genreIds,
    [FromQuery] PaginationDTO pagination)
        {
            if (genreIds <= 0)
            {
                return BadRequest("Invalid genre ID provided.");
            }

            var queryable = _context.Movies
                .Include(m => m.MoviesGenres)
                .ThenInclude(mg => mg.Genre)
                .AsQueryable();


            queryable = queryable.Where(m =>
                m.MoviesGenres.Any(mg => mg.GenreId == genreIds));

            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);

          
            var movies = await queryable.Paginate(pagination).ToListAsync();

           
            var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

     
            return Ok(movieDTOs);
        }

        [HttpGet("by-genre name")]
        [SwaggerOperation(
            Summary = "Get movies by genre names",
            Description = "Retrieves a list of movies that match the specified genre names. Returns a list of movies filtered according to the provided genre names."
        )]
        public async Task<ActionResult<List<MovieDTO>>> GetMoviesByGenres([FromQuery] Genrefiltername filterDto)
        {
            var queryable = _context.Movies.AsQueryable();

            if (filterDto.GenreNames != null && filterDto.GenreNames.Any())
            {
 
                queryable = queryable
                    .Where(m => m.MoviesGenres
                        .Any(mg => filterDto.GenreNames.Contains(mg.Genre.Name)));
            }


            queryable = queryable
                .Include(m => m.MoviesGenres)
                .ThenInclude(mg => mg.Genre);

            var movies = await queryable.ToListAsync();


            var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);


            foreach (var movieDto in movieDTOs)
            {
                var movieEntity = movies.FirstOrDefault(m => m.Id == movieDto.Id);
                if (movieEntity != null)
                {
                    movieDto.Poster = movieEntity.PosterUrl;
                }
                else
                {
                    movieDto.Poster = null;
                }
            }

            return Ok(movieDTOs);
        }

        
    }
}
