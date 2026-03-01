using FluentValidation;
using DentalHub.Application.Commands.Doctor;

namespace DentalHub.Application.Validators.Doctors
{
    public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
    {
        public UpdateDoctorCommandValidator()
        {
            RuleFor(x => x.PublicId)
                .NotEmpty().WithMessage("Doctor public ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(3, 100).WithMessage("Name must be between 3 and 100 characters")
                .Matches(@"^[a-zA-Z\u0621-\u064A\s]+$").WithMessage("Name can only contain letters and spaces");

            RuleFor(x => x.Specialty)
                .NotEmpty().WithMessage("Specialty is required")
                .Length(3, 100).WithMessage("Specialty must be between 3 and 100 characters");

            RuleFor(x => x.UniversityId)
                .GreaterThan(0).WithMessage("University ID is required");
        }
    }
}
