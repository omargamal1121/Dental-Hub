using FluentValidation;
using DentalHub.Application.Commands.CaseRequests;

namespace DentalHub.Application.Validators.CaseRequests
{
    public class ApproveCaseRequestCommandValidator : AbstractValidator<ApproveCaseRequestCommand>
    {
        public ApproveCaseRequestCommandValidator()
        {
            RuleFor(x => x.RequestPublicId)
                .NotEmpty().WithMessage("Request public ID is required");

            RuleFor(x => x.DoctorPublicId)
                .NotEmpty().WithMessage("Doctor public ID is required");
        }
    }
}
