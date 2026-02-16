using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Domain.Entities
{
	public class UniversityMember
	{
		public int Id { get; set; }
		


		public string UniversityId { get; set; } = null!;
		public string FullName { get; set; } = null!;
		public string Faculty { get; set; } = null!;
		public string Department { get; set; } =null ;
        public bool IsDoctor { get; set; } = false;
        public bool IsStudent { get; set; } = false;

        public string Role => IsDoctor ? "Doctor" : IsStudent ? "Student" : "Unknown";
    }
}
