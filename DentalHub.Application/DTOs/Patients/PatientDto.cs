using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.DTOs.Patients
{
    /// DTO for patient information
    public class PatientDto
    {
        public string PublicId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime CreateAt { get; set; }
       
        public List<PatientCaseSimpleDataDto> PatientCases { get; set; }
		
	}
	

}
