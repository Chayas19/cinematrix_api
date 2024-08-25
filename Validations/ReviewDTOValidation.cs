using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class ReviewDTOValidation : AbstractValidator<ReviewDTO>
    {

        public ReviewDTOValidation() 
        {
            RuleFor(review => review.MovieId)
             .GreaterThan(0).WithMessage("MovieId  must be greater than zero");

            RuleFor(review => review.MovieTitle)
             .NotEmpty().WithMessage("MovieTitle is required")
              .Length(1, 200).WithMessage("MovieTitle must be between 1 and 200 characters");

            RuleFor(review => review.UserId)
                  .GreaterThan(0).WithMessage("UserId  must be greater than zero");

            RuleFor(review => review.Content)
                .NotEmpty().WithMessage("Content is required")
                .Length(1, 1000).WithMessage("Content must be between 1 and 1000 charcaters ");

            RuleFor(review => review.Rating)
                .InclusiveBetween(1, 5).WithMessage("Raing must be between 1 and 5");


        }    
    }
}
