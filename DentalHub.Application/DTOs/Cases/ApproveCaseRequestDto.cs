namespace DentalHub.Application.DTOs.Cases
{
    public class ApproveCaseRequestDto
    {
        public Guid RequestId { get; set; } 

        public Guid DoctorId { get; set; } 

        public bool IsApproved { get; set; }

        public string? RejectionReason { get; set; }
    }
}
