using FuzzySharp;  // Fuzzy matching library
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMatrix_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalSearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GlobalSearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
          
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query parameter is required.");
            }
 
            string lowerQuery = query.ToLower();

           
            var movies = await _context.Movies
                .Include(m => m.MoviesGenres)    
                .Include(m => m.MovieLanguages) 
                .Include(m => m.MoviesActors)     
                .ToListAsync();
            var actors = await _context.Actors.ToListAsync();
            var genres = await _context.Genres.ToListAsync();
            var languages = await _context.Languages.ToListAsync();


            int threshold = 80;

           
            var genreResults = genres
                .Where(g => Fuzz.Ratio(g.Name.ToLower(), lowerQuery) >= threshold)  
                .SelectMany(g => _context.MovieGenres
                    .Where(mg => mg.GenreId == g.Id)   
                    .Select(mg => new
                    {
                        Type = "Movie",
                        Name = mg.Movie.Title,
                        Genre = g.Name
                    })
                ).ToList();

      
            var actorResults = actors
                .Where(a => Fuzz.Ratio(a.Name.ToLower(), lowerQuery) >= threshold)  
                .SelectMany(a => _context.MovieActors
                    .Where(ma => ma.ActorId == a.Id)   
                    .Select(ma => new
                    {
                        Type = "Movie",
                        Name = ma.Movie.Title,
                        Actor = a.Name
                    })
                ).ToList();


            var languageResults = languages
                .Where(l => Fuzz.Ratio(l.Name.ToLower(), lowerQuery) >= threshold)  
                .SelectMany(l => _context.MoviesLanguages
                    .Where(ml => ml.LanguageId == l.Id)   
                    .Select(ml => new
                    {
                        Type = "Movie",
                        Name = ml.Movie.Title,
                        Language = l.Name
                    })
                ).ToList();


            var movieResults = movies
                .Where(m => m.Title.ToLower().Contains(lowerQuery))
                .Select(m => new
                {
                    Type = "Movie",
                    Name = m.Title,
                    Genre = string.Join(", ", m.MoviesGenres.Select(mg => mg.Genre.Name)),
                    Language = string.Join(", ", m.MovieLanguages.Select(ml => ml.Language.Name))
                })
                .ToList();

      
            var allResults = genreResults
                .Select(g => new { g.Type, g.Name, Genre = g.Genre, Actor = (string)null, Language = (string)null })
                .Union(
                    actorResults.Select(a => new { a.Type, a.Name, Genre = (string)null, Actor = a.Actor, Language = (string)null })
                )
                .Union(
                    languageResults.Select(l => new { l.Type, l.Name, Genre = (string)null, Actor = (string)null, Language = l.Language })
                )
                .Union(
                    movieResults.Select(m => new { m.Type, m.Name, Genre = m.Genre, Actor = (string)null, Language = m.Language })
                )
                .Distinct()  
                .ToList();


            if (!allResults.Any())
            {
                return NotFound("No results found.");
            }

            return Ok(allResults);
        }
    }
}
