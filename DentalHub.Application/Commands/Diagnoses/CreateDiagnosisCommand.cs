using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Domain.Entities;
using MediatR;

namespace DentalHub.Application.Commands.Diagnoses
{
    public class CreateDiagnosisCommand : IRequest<Result<DiagnosisDto>>
    {
        public Guid PatientCaseId { get; set; }
        public DiagnosisStage Stage { get; set; }
        public Guid CaseTypeId { get; set; }
        public string Notes { get; set; }
        public Guid? CreatedById { get; set; }
        public string Role { get; set; }
        public List<int>? TeethNumbers { get; set; }

        public CreateDiagnosisDto ToDto()
        {
            return new CreateDiagnosisDto
            {
                PatientCaseId = this.PatientCaseId,
                Stage = this.Stage,
                CaseTypeId = this.CaseTypeId,
                Notes = this.Notes,
               
                TeethNumbers = this.TeethNumbers
            };
        }
    }
}
