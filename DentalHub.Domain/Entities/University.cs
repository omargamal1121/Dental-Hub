using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Domain.Entities
{
    public class University : BaseEntitiy
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<UniversityMember> Members { get; set; } = new List<UniversityMember>();

        public static implicit operator University(string v)
        {
            throw new NotImplementedException();
        }
    }
}
