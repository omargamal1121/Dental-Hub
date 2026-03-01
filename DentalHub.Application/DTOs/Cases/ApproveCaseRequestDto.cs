namespace DentalHub.Application.DTOs.Cases
{
    public class ApproveCaseRequestDto
    {
        public string RequestId { get; set; } = string.Empty;

        public string DoctorId { get; set; } = string.Empty;

        public bool IsApproved { get; set; }

        public string? RejectionReason { get; set; }
    }
}
