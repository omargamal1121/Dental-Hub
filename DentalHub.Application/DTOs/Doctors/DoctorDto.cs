using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Doctors
{
    public class DoctorDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string UniversityId { get; set; }

        public DateTime CreateAt { get; set; }
        public int TotalStudents { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
    }
  
}
