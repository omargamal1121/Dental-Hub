namespace DentalHub.Application.DTOs.Students
{
	public class StudentDetailsDto : StudentDto
    {

        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int TotalSessions { get; set; }
    }
}