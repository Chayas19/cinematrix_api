using AutoMapper;
using CineMatrix_API;
using CineMatrix_API.DTOs;
using CineMatrix_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class MoviesByLanguages : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public MoviesByLanguages(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost("addMovie")]
    public async Task<ActionResult> AddMovie([FromBody] MovieCreationDTO movieDto, [FromQuery] string languageName)
    {
        if (movieDto == null)
        {
            return BadRequest("MovieCreationDTO is null.");
        }

      
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Name.Equals(languageName, StringComparison.OrdinalIgnoreCase));
        if (language == null)
        {
            return BadRequest("The specified language does not exist.");
        }

   
        var movie = new Movie
        {
            Title = movieDto.Title
        };

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

   
        var movieLanguage = new MovieLanguage
        {
            MovieId = movie.Id,
            LanguageId = language.Id
        };

        _context.MoviesLanguages.Add(movieLanguage);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new
        {
            movieId = movie.Id,
            languageId = language.Id
        }, movie);
    }

    [HttpGet("movies/{languageName}")]
    public async Task<ActionResult<List<MovieDTO>>> GetMoviesByLanguageName(string languageName)
    {
        
        var language = await _context.Languages
            .Where(l => l.Name.ToLower() == languageName.ToLower())
            .FirstOrDefaultAsync();

        if (language == null)
        {
            return BadRequest("The specified language does not exist.");
        }
 
      
        var movies = await _context.MoviesLanguages
            .Where(ml => ml.LanguageId == language.Id)
            .Include(ml => ml.Movie) 
                .ThenInclude(m => m.MoviesGenres) 
                    .ThenInclude(mg => mg.Genre)
            .Select(ml => ml.Movie)
            .Distinct()
            .ToListAsync();


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



  

    [HttpGet("{movieId}/{languageId}")]
    public async Task<ActionResult<MovielanguageDTO>> GetById(int movieId, int languageId)
    {
        var movieLanguage = await _context.MoviesLanguages
            .Include(ml => ml.Movie)
            .Include(ml => ml.Language)
            .FirstOrDefaultAsync(ml => ml.MovieId == movieId && ml.LanguageId == languageId);

        if (movieLanguage == null)
        {
            return NotFound();
        }

        var movieLanguageDto = _mapper.Map<MovielanguageDTO>(movieLanguage);
        return Ok(movieLanguageDto);
    }
}
