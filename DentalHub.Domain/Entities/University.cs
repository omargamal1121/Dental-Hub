using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Domain.Entities
{
	public class University : BaseEntitiy
	{
		[Required(ErrorMessage ="Name Needed")]
		public string Name { get; set; } = null!;

		[EmailAddress]
		public string Email { get; set; } = null!;
		[Phone]

		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public int? FoundedYear { get; set; }
		
		public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
		public ICollection<Student> Students { get; set; } = new List<Student>();
		public ICollection<UniversityMember> Members { get; set; } = new List<UniversityMember>();
	}
}
