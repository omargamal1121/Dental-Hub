using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.Doctors
{
    public class DoctorSessionsSpec : BaseSpecification<Session>
    {
        public DoctorSessionsSpec(Guid doctorId)
            : base(s => s.DoctorId == doctorId)
        {
            AddInclude(s => s.PatientCase);
            AddInclude(s => s.Student);
        }
    }
}