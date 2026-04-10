using DentalHub.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Diagnoses
{
    public class CreateDiagnosisDto
    {
        [Required]
        public Guid PatientCaseId { get; set; }

        [Required]
        public DiagnosisStage Stage { get; set; }

        [Required]
        public Guid CaseTypeId { get; set; }

        public string Notes { get; set; }

      
        public List<int>? TeethNumbers { get; set; }
    }
}
