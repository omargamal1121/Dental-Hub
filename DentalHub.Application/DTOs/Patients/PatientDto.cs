using DentalHub.Application.DTOs.Cases;
using DentalHub.Domain.Entities;

namespace DentalHub.Application.DTOs.Patients
{
    /// DTO for patient information
    public class PatientDto
    {
        public Guid PublicId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Age { get; set; }
        public string NationalId { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public City City { get; set; }
        public Gender Gender { get; set; }
       
        public List<PatientCaseSimpleDataDto> PatientCases { get; set; }
		
	}
	

}
