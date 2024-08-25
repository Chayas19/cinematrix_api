using CineMatrix_API.DTOs;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace CineMatrix_API.Validations
{
    public class MoviecreationDTOValidators : AbstractValidator<MovieCreationDTO>
    {
        public MoviecreationDTOValidators()
        {
            RuleFor(x => x.Title)
             .NotEmpty().WithMessage("Title is required");

            RuleFor(x => x.Description)
             .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.Duration)
             .NotEmpty().WithMessage("Duration is required")
             .Matches(@"^\d{2}:\d{2}$").WithMessage("Duration must be in hh:mm format.");

            RuleFor(x => x.Director)
               .NotEmpty().WithMessage("Director is required");

            RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.");

            //RuleFor(x => x.PosterUrl)
            //    .NotEmpty().WithMessage("Poster URL is required.")
            //    .Must(BeAValidUrl).WithMessage("Poster URL is not valid.");

            RuleFor(x => x.SubscriptionType)
                .IsInEnum().WithMessage("Invalid subscription type.");

            RuleFor(x => x.GenresIds)
                .NotEmpty().WithMessage("At least one genre ID is required.");

            //RuleFor(x => x.Actors)
            //    .NotEmpty().WithMessage("At least one actor is required.")
            //    .ForEach(actor => actor
            //        .SetValidator(new ActorCreationValidation())
            //    );

        }
    }
}
