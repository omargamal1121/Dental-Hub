namespace DentalHub.Application.Services.Doctors
{
	/// DTO for doctor statistics
	public class DoctorStatsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalStudents { get; set; }
        public int ActiveCases { get; set; }
        public int CompletedCases { get; set; }
    }
}
