using System;
using System.Collections.Generic;

namespace DentalHub.Domain.Entities
{
    public class Patient
    {
        public Guid UserId { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }

        // Navigation property
        public User User { get; set; }

        // One-to-Many: Patient -> PatientCases
        public ICollection<PatientCase> PatientCases { get; set; } = new List<PatientCase>();
    }
}
