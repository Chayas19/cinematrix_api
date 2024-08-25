using CineMatrix_API.DTOs;
using CineMatrix_API.Enums;
using FluentValidation;

namespace CineMatrix_API.Validations
{
    public class UserRolesDTOValidation : AbstractValidator<UserRolesDTO>
    {
        public UserRolesDTOValidation()
        {
            RuleFor(dto => dto.UserId)
                    .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(dto => dto.RoleName)
                    .NotEmpty().WithMessage("RoleName is required.")
                    .Must(roleName => Enum.IsDefined(typeof(RoleType), roleName)).WithMessage("Invalid RoleName specified.");
        }
    }
}
