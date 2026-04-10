using DentalHub.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Diagnoses
{
    public class UpdateDiagnosisDto
    {
        [Required]
        public Guid Id { get; set; }

        public DiagnosisStage? Stage { get; set; }

        public Guid? CaseTypeId { get; set; }

        public string? Notes { get; set; }
        public List<int>? TeethNumbers { get; set; }
    }
}
