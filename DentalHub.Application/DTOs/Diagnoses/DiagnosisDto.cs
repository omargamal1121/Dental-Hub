using DentalHub.Domain.Entities;

namespace DentalHub.Application.DTOs.Diagnoses
{
    public class DiagnosisDto
    {
        public Guid Id { get; set; }
        public Guid PatientCaseId { get; set; }
        public DiagnosisStage Stage { get; set; }
        public Guid CaseTypeId { get; set; }
        public string CaseTypeName { get; set; }
        public string Notes { get; set; }
        public Guid? CreatedById { get; set; }
        public string Role { get; set; }
        public bool? IsAccepted { get; set; }
        public List<int> TeethNumbers { get; set; } = new List<int>();
    }
}
