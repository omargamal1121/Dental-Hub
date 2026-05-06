using FluentValidation;
using DentalHub.Application.Commands.v2.PatientCase;

namespace DentalHub.Application.Validators.PatientCase.v2
{
    public class CreatePatientCaseV2CommandValidator : AbstractValidator<CreatePatientCaseV2Command>
    {
        public CreatePatientCaseV2CommandValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .Length(14).WithMessage("National ID must be 14 digits.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            When(x => x.InitialDiagnosis != null, () =>
            {
                RuleFor(x => x.InitialDiagnosis!.CaseTypeId)
                    .NotEmpty().WithMessage("Diagnosis Case Type is required.");

                RuleFor(x => x.InitialDiagnosis!.Notes)
                    .NotEmpty().WithMessage("Diagnosis notes are required when adding a diagnosis.");
                
                RuleFor(x => x.InitialDiagnosis!.Stage)
                    .IsInEnum().WithMessage("Invalid diagnosis stage.");
            });
        }
    }
}
