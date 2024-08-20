using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMatrix_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Globalsearch : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Globalsearch(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string letter, [FromQuery] string type)
        {
            if (string.IsNullOrEmpty(letter) || string.IsNullOrEmpty(type))
            {
                return BadRequest("Letter and type are required.");
            }

 
            string lowerLetter = letter.ToLower();
            IQueryable<object> results;

            switch (type.ToLower())
            {
                case "movie":
                    results = _context.Movies
                        .Where(m => m.Title.ToLower().StartsWith(lowerLetter))
                        .Select(m => new { m.Title });
                    break;

                case "actor":
                    results = _context.Actors
                        .Where(a => a.Name.ToLower().StartsWith(lowerLetter))
                        .Select(a => new { a.Name });
                    break;

             
                case "language":
                    results = _context.Languages
                        .Where(l => l.Name.ToLower().StartsWith(lowerLetter))
                        .Select(l => new { l.Name });
                    break;

                case "genre":
                    results = _context.Genres
                        .Where(g => g.Name.ToLower().StartsWith(lowerLetter))
                        .Select(g => new { g.Name });
                    break;

                default:
                    return BadRequest("Invalid type specified.");
            }

            var resultList = await results.ToListAsync();
            return Ok(resultList);
        }
    }
}
