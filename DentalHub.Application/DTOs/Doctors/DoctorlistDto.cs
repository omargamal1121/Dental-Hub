namespace DentalHub.Application.DTOs.Doctors
{
	public class DoctorlistDto
	{
		public string PublicId { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Specialty { get; set; } = string.Empty;
		public string UniversityId { get; set; } = null!;

		public DateTime CreateAt { get; set; }
	}
  
}
