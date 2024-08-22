using CineMatrix_API.DTOs;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class PaginationDTOValidator : AbstractValidator<PaginationDTO>
    {

        public PaginationDTOValidator()
        {
            RuleFor(x => x.Page)
            .NotEmpty().WithErrorCode("page no is required")
            .GreaterThan(0).WithMessage("Page no must be greater than zero");

            RuleFor(x => x.RecordsPerPage)
              .NotEmpty().WithMessage("RecordsPerPage is required")
              .GreaterThan(0).WithMessage("Records per page must be greater than 0.")
              .LessThanOrEqualTo(50).WithMessage("Records per page cannot exceed 50.");
        }
    }
}
