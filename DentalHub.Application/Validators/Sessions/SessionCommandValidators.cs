using FluentValidation;
using DentalHub.Application.Commands.Sessions;

namespace DentalHub.Application.Validators.Sessions
{
    public class UpdateSessionStatusCommandValidator : AbstractValidator<UpdateSessionStatusCommand>
    {
        public UpdateSessionStatusCommandValidator()
        {
            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("Session ID is required");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required");
        }
    }

    public class AddSessionNoteCommandValidator : AbstractValidator<AddSessionNoteCommand>
    {
        public AddSessionNoteCommandValidator()
        {
            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("Session ID is required");

            RuleFor(x => x.Note)
                .NotEmpty().WithMessage("Note content is required")
                .Length(10, 2000).WithMessage("Note must be between 10 and 2000 characters");
        }
    }
}
