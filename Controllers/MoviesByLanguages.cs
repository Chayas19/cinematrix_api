using AutoMapper;
using CineMatrix_API;
using CineMatrix_API.DTOs;
using CineMatrix_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

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

    [HttpGet("movies/{languageName}")]
    [SwaggerOperation(
        Summary = "Get movies by language name",
        Description = "Retrieves a list of movies that are available in the specified language. If the language does not exist, returns a BadRequest. If no movies are found for the specified language, returns a NotFound response."
    )]
    public async Task<ActionResult<List<MovieDTO>>> GetMoviesByLanguageName(string languageName)
    {
        var languageId = await _context.Languages
            .Where(l => l.Name.ToLower() == languageName.ToLower())
            .Select(l => l.Id)
            .FirstOrDefaultAsync();

        if (languageId == 0)
        {
            return BadRequest("The specified language does not exist.");
        }

        var movies = await _context.MoviesLanguages
            .Where(ml => ml.LanguageId == languageId)
            .Select(ml => ml.Movie)
            .Distinct()
            .ToListAsync();

        if (movies == null || !movies.Any())
        {
            return NotFound("No movies found for the specified language.");
        }

        var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

        foreach (var movieDto in movieDTOs)
        {
            var movieEntity = movies.FirstOrDefault(m => m.Id == movieDto.Id);
            movieDto.Poster = movieEntity?.PosterUrl;
        }

        return Ok(movieDTOs);
    }



}
