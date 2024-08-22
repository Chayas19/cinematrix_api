using AutoMapper;
using CineMatrix_API;
using CineMatrix_API.DTOs;
using CineMatrix_API.Filters;
using CineMatrix_API.Helpers;
using CineMatrix_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Orders;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

[Route("api/[controller]")]
[ApiController]
public class ActorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ActorsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    //actor
    [SwaggerOperation(Summary = "Get a list of actors",
                          Description = "Retrieves all the list of actors")]
    public async Task<IActionResult> Get([FromQuery] PaginationDTO pagination)
    {
        try
        {
            var queryable = _context.Movies.AsQueryable();

            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);

            var movies = await queryable.Paginate(pagination).ToListAsync();

            var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

   
            var baseUrl = $"{Request.Scheme}://{Request.Host}/images/";

            foreach (var movieDTO in movieDTOs)
            {
                if (movieDTO.PosterData != null && movieDTO.PosterData.Length > 0)
                {
                    movieDTO.ImageUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(movieDTO.PosterData)}";
                }
                else if (!string.IsNullOrEmpty(movieDTO.Poster))
                {
                    movieDTO.ImageUrl = $"{baseUrl}{movieDTO.Poster}";
                }
                else
                {
              
                    movieDTO.ImageUrl = $"{baseUrl}default.jpg"; 
                }
            }

            return Ok(new { success = true, message = "Movies retrieved successfully.", data = movieDTOs });
        }
        catch (Exception ex)
        {
         

            return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An error occurred while fetching movies. Please try again later." });
        }
    }

    [HttpGet("{id}", Name = "getactor")]
    public async Task<ActionResult<PersonDTO>> GetById(int id)
    {
        var actor = await _context.Actors.Include(a => a.MoviesActors)
                                         .FirstOrDefaultAsync(a => a.Id == id);
        if (actor == null)
        {
            return NotFound(new { message = $"Actor wth ID {id} not found" });

        }
        var actordto = _mapper.Map<PersonDTO>(actor);
        if (actordto.PictureUrl != null && actordto.PictureUrl.Length > 0)
        {

            actordto.PictureUrl = $"{Convert.ToBase64String(actor.Picture)}";

        }
        else
        {
            actordto.PictureUrl = "null";
        }

        return Ok(new { success = true, data = actordto }); 
    }


        [HttpPost]
    // [Authorize(Roles = "Admin")]

    public async Task<ActionResult> Post([FromForm] PersonCreationDTO personCreationDto)
    {
        if (personCreationDto.Picture != null && personCreationDto.Picture.Length > 0)
        {
            var existingActor = await _context.Actors
                .FirstOrDefaultAsync(a => a.Name == personCreationDto.Name);

            if (existingActor != null)
            {
                return Conflict(new { message = "An actor with this name already exists." });
            }

            var pictureUrl = await SaveFileAsync(personCreationDto.Picture);

    
            var dateOfBirth = DateOnly.Parse(personCreationDto.DateOfBirth);

            var actor = new Actor
            {
                Name = personCreationDto.Name,
                Biography = personCreationDto.Biography,
                Dateofbirth = dateOfBirth,
                PictureUrl = pictureUrl
            };

            _context.Actors.Add(actor);
           // await _context.SaveChangesAsync();

            var actorDto = _mapper.Map<ActorDTO>(actor);

            return CreatedAtRoute("getactor", new { id = actor.Id }, new
            {
                message = "Actor created successfully!",
                actor = actorDto
            });
        }

        return BadRequest(new { message = "No file uploaded or file is empty." });
    }

    [HttpPut("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateActor(int id, [FromForm] PersonCreationDTO personUpdateDto)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null)
        {
            return NotFound(new { message = $"Actor with ID {id} not found." });
        }

        actor.Name = personUpdateDto.Name ?? actor.Name;
        actor.Biography = personUpdateDto.Biography ?? actor.Biography;

        if (!string.IsNullOrEmpty(personUpdateDto.DateOfBirth))
        {
            actor.Dateofbirth = DateOnly.Parse(personUpdateDto.DateOfBirth);
        }

        if (personUpdateDto.Picture != null)
        {
            var pictureUrl = await SaveFileAsync(personUpdateDto.Picture);

            if (!string.IsNullOrEmpty(actor.PictureUrl))
            {
                var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", actor.PictureUrl);
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }

            actor.PictureUrl = pictureUrl;
        }

        _context.Entry(actor).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }



    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteActor(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null)
        {
            return NotFound(new { message = $"Actor with ID {id} not found." });
        }

        if (!string.IsNullOrEmpty(actor.PictureUrl))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", actor.PictureUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Actors.Remove(actor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    private async Task<string> SaveFileAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var timestamp = DateTime.Now.ToString("ddMMyyyyy");
        var fileExtension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{timestamp}{fileExtension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        // Return the relative path
        return Path.Combine("images", uniqueFileName).Replace("\\", "/");
    }



    [HttpGet("movies-by-actor/{actorId}")]
    public async Task<ActionResult<List<MovieDTO>>> GetMoviesByActor(int actorId)
    {
        var movies = await _context.MovieActors
            .Where(ma => ma.ActorId == actorId)
            .Select(ma => ma.Movie)
            .Distinct()
            .ToListAsync();

        if (movies == null || movies.Count == 0)
        {
            return NotFound(new { message = $"No movies found for actor with ID {actorId}." });
        }

        var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);
        return Ok(movieDTOs);
    }

    [HttpGet("movies-by-actor-name")]
    public async Task<ActionResult<List<MovieDTO>>> GetMoviesByActorName([FromQuery] string actorName)
    {
        if (string.IsNullOrWhiteSpace(actorName))
        {
            return BadRequest(new { message = "Actor name cannot be empty." });
        }

        var actorIds = await _context.Actors
            .Where(a => a.Name.Contains(actorName))
            .Select(a => a.Id)
            .ToListAsync();

        if (actorIds == null || actorIds.Count == 0)
        {
            return NotFound(new { message = $"No actors found with name containing '{actorName}'." });
        }

        var movies = await _context.MovieActors
            .Where(ma => actorIds.Contains(ma.ActorId))
            .Select(ma => ma.Movie)
            .Distinct()
            .ToListAsync();

        if (movies == null || movies.Count == 0)
        {
            return NotFound(new { message = $"No movies found for actors with name containing '{actorName}'." });
        }

        var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);
        return Ok(movieDTOs);
    }
}
