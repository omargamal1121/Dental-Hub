using FluentValidation;
using DentalHub.Application.Features.Admins.Commands.CreateAdmin;
using DentalHub.Application.Features.Admins.Commands.UpdateAdmin;

namespace DentalHub.Application.Validators.Admins
{
    public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
    {
        public CreateAdminCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .Length(3, 100).WithMessage("Full name must be between 3 and 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .Length(6, 100).WithMessage("Password must be between 6 and 100 characters");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .MaximumLength(100).WithMessage("Role cannot exceed 100 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^\+?[1-9]\d{7,14}$").WithMessage("Invalid phone number format");
        }
    }

    public class UpdateAdminCommandValidator : AbstractValidator<UpdateAdminCommand>
    {
        public UpdateAdminCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Admin public ID is required");

            RuleFor(x => x.FullName)
                .Length(3, 100).When(x => !string.IsNullOrEmpty(x.FullName)).WithMessage("Full name must be between 3 and 100 characters");

            RuleFor(x => x.Role)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Role)).WithMessage("Role cannot exceed 100 characters");

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[1-9]\d{7,14}$").When(x => !string.IsNullOrEmpty(x.Phone)).WithMessage("Invalid phone number format");
        }
    }
}
