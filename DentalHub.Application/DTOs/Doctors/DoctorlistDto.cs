namespace DentalHub.Application.DTOs.Doctors
{
	public class DoctorlistDto
	{
		public Guid PublicId { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Specialty { get; set; } = string.Empty;
		public Guid UniversityId { get; set; }

		public DateTime CreateAt { get; set; }
	}
  
}
