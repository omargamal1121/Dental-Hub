using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Domain.Entities;
using MediatR;

namespace DentalHub.Application.Commands.Diagnoses
{
    public class UpdateDiagnosisCommand : IRequest<Result<DiagnosisDto>>
    {
        public Guid Id { get; set; }
        public DiagnosisStage? Stage { get; set; }
        public Guid? CaseTypeId { get; set; }
        public string? Notes { get; set; }
        public List<int>? TeethNumbers { get; set; }

        public UpdateDiagnosisDto ToDto()
        {
            return new UpdateDiagnosisDto
            {
                Id = this.Id,
                Stage = this.Stage,
                CaseTypeId = this.CaseTypeId,
                Notes = this.Notes,
                TeethNumbers = this.TeethNumbers
            };
        }
    }
}
