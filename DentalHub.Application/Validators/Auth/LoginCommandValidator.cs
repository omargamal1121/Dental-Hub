using FluentValidation;
using DentalHub.Application.Commands.Auth;

namespace DentalHub.Application.Validators.Auth
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
         

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");
        }
    }
}
