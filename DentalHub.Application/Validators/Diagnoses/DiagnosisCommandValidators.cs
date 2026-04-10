using FluentValidation;
using DentalHub.Application.Commands.Diagnoses;

namespace DentalHub.Application.Validators.Diagnoses
{
    public class CreateDiagnosisCommandValidator : AbstractValidator<CreateDiagnosisCommand>
    {
        public CreateDiagnosisCommandValidator()
        {
            RuleFor(x => x.PatientCaseId).NotEmpty();
            RuleFor(x => x.CaseTypeId).NotEmpty();
            RuleFor(x => x.Stage).IsInEnum();
            RuleFor(x => x.Notes).MaximumLength(1000);
        }
    }

    public class UpdateDiagnosisCommandValidator : AbstractValidator<UpdateDiagnosisCommand>
    {
        public UpdateDiagnosisCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Notes).MaximumLength(1000).When(x => x.Notes != null);
        }
    }
}
