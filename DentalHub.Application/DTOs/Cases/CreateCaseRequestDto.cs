namespace DentalHub.Application.DTOs.Cases
{
    public class CreateCaseRequestDto
    {
        public Guid PatientCasePublicId { get; set; } 

        public Guid StudentPublicId { get; set; } 

        public Guid DoctorPublicId { get; set; } 

        public string Description { get; set; } = string.Empty;
    }
}
