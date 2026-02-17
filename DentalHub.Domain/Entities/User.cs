using Microsoft.AspNetCore.Identity;
using System;

namespace DentalHub.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string PublicId { get; set; } = string.Empty;

        public User()
        {
            Id = Guid.CreateVersion7();
            PublicId = DentalHub.Domain.Utils.Base62Converter.Encode(Id);
        }

        public string FullName { get; set; }
        public Patient? Patient { get; set; }
        public Student? Student { get; set; }
        public Doctor? Doctor { get; set; }
        public Admin? Admin { get; set; }
		public ICollection<IdentityUserRole<Guid>> UserRoles{ get; set; }
	}
}
