namespace DentalHub.Application.Services.Students
{
	/// DTO for student statistics
	public class StudentStatsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int TotalCases { get; set; }
    }
}
