using System;

namespace DentalHub.Domain.Entities
{
    public class Doctor
    {
        public Guid UserId { get; set; } // Primary key + FK
        public string Specialty { get; set; }

        public User User { get; set; }
    }
}
