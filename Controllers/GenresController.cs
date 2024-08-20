using AutoMapper;
using CineMatrix_API.DTOs;
using CineMatrix_API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMatrix_API.Controllers
{
    [Route("api/genres")] 
    [ApiController] 
    public class GenericController : ControllerBase
    {
        private readonly ILogger<GenericController> _logger; 
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper; 

        public GenericController(ILogger<GenericController> logger,
            ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        // GET: api/genres
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
           
            var genres = await _context.Genres.ToListAsync();

         
            var genreDTOs = _mapper.Map<List<GenreDTO>>(genres);

          
            return genreDTOs;
        }

        // GET: api/genres/{id}
        [HttpGet("{id:int}", Name = "getGenre")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<GenreDTO>> GetByID(int id)
        {
        
            var genre = await _context.Genres.FindAsync(id);

        
            if (genre == null)
            {
                return NotFound();
            }

        
            var genreDTO = _mapper.Map<GenreDTO>(genre);

         
            return genreDTO;
        }

        // POST: api/genres
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDto)
        {
            
            var existingGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreCreationDto.Name);

            if (existingGenre != null)
            {
                
                return Conflict(new { message = "Genre with the name already exists." });
            }

       
            var genre = _mapper.Map<Genre>(genreCreationDto);

            
            _context.Genres.Add(genre);

            
            await _context.SaveChangesAsync();

        
            var genreDto = _mapper.Map<GenreDTO>(genre);

     
            return CreatedAtRoute("getGenre", new { Id = genre.Id }, new
            {
                message = "Genre created successfully!", 
                genre = genreDto
            });
        }

        // PUT: api/genres/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreDto)
        {
          
            var existingGenre = await _context.Genres.FindAsync(id);


            if (existingGenre == null)
            {
                return NotFound("Genre not found.");
            }

            
            var existingData = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreDto.Name && g.Id != id);

            if (existingData != null)
            {
                return Conflict(new { message = "Unable to update the genre because a genre with that name already exists. Please choose a different name." });
            }

         
            _mapper.Map(genreDto, existingGenre);

            try
            {
             
                _context.Entry(existingGenre).State = EntityState.Modified;

              
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
             
                if (!GenreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; 
                }
            }

          
            _logger.LogInformation("Updated genre with ID: {Id}", id);

          
            return NoContent();
        }

        // DELETE: api/genres/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
          
            var genre = await _context.Genres.FindAsync(id);

         
            if (genre == null)
            {
                return NotFound();
            }

         
            _context.Genres.Remove(genre);

     
            await _context.SaveChangesAsync();

          
            return NoContent();
        }


        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}
