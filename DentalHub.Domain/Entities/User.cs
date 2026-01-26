using Microsoft.AspNetCore.Identity;
using System;

namespace DentalHub.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public UserRole Role { get; set; }

        // Navigation properties
        public Patient Patient { get; set; }
        public Student Student { get; set; }
        public Doctor Doctor { get; set; }
    }
}
