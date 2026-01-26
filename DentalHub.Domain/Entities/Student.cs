using System;

namespace DentalHub.Domain.Entities
{
    public class Student
    {
        public Guid UserId { get; set; } // Primary key + FK
        public string University { get; set; }
        public int Level { get; set; }

        public User User { get; set; }
    }
}
