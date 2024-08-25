using System.Linq;
using CineMatrix_API.DTOs;
using CineMatrix_API.Helpers;
using CineMatrix_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CineMatrix_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reviews
        [HttpGet("get-reviews")]
        [SwaggerOperation(Summary = "Get all reviews",
                          Description = "Retrieves a list of all reviews with movie and user details.")]
        public async Task<IActionResult> GetReviews([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var queryable = _context.Reviews
                    .Include(r => r.Movie)
                    .Include(r => r.User)
                    .AsQueryable();

                var reviews = await queryable
                    .Paginate(pagination)
                    .ToListAsync();

                if (reviews == null || !reviews.Any())
                {
                    return NotFound(new { success = false, message = "No reviews found." });
                }

                var reviewDTOs = reviews.Select(r => new
                {
                    MovieTitle = r.Movie.Title,
                    Content = r.Content,
                    UserName = r.User.Name
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Reviews retrieved successfully.",
                    data = reviewDTOs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while fetching reviews. Please try again later."
                });
            }
        }


        // GET: api/reviews/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieve a review by ID",
                          Description = "Retrieves a review with associated movie and user details by review ID.")]
        public async Task<IActionResult> GetReview(int id)
        {
            var review = await _context.Reviews.Include(r => r.Movie)
                                               .Include(r=> r.User)
                                               .FirstOrDefaultAsync(r => r.Id == id);   

            if (review == null)
            {
                return NotFound(new { success = false, message = "Review not found." });
            }

            var reviewDTO = new
            {
                MovieTitle = review.Movie.Title,
                review = review.Content,
                UserName = review.User.Name

            };
            return Ok(new {success = true, data = reviewDTO ,
             message = "Data retrieved successfully"
            }); 
        }

        // POST: api/reviews
        [HttpPost]
        // [Authorize(Roles = "PrimeUser,Admin")]
        [SwaggerOperation(Summary = "Create a new review",
                          Description = "Allows any authenticated user to create a review for a movie.")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDTO reviewDTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid review data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var review = new Reviews
                {
                    MovieId = reviewDTO.MovieId,
                    UserId = reviewDTO.UserId,
                    Content = reviewDTO.Content,
                    Rating = reviewDTO.Rating
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, new
                {
                    success = true,
                    data = review,
                    message = "Review created successfully."
                });
            }
            catch (DbUpdateException dbEx)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while updating the database. Please try again later."
                });
            }
            catch (Exception ex)
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }




        // PUT: api/reviews/{id}
        [HttpPut("{id}")]
        // [Authorize(Roles = "PrimeUser,Admin")]
        [SwaggerOperation(Summary = "Update an existing review",
                          Description = "Updates an existing review with new data.")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid review data.", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound(new { success = false, message = "Review not found." });
            }

            review.MovieId = reviewDTO.MovieId;
            review.UserId = reviewDTO.UserId;
            review.Content = reviewDTO.Content;
            review.Rating = reviewDTO.Rating;

            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/reviews/{id}
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a review by ID",
                          Description = "Deletes a review identified by its ID.")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound(new { success = false, message = "Review not found." });
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
