namespace DentalHub.Application.DTOs.Cases
{
    public class CreateCaseRequestDto
    {
        public Guid PatientCasePublicId { get; set; } 

        public Guid StudentPublicId { get; set; }

        public string DoctorUsername { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
