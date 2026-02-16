using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.Doctors
{
    public class DoctorDashboardSpec : BaseSpecification<CaseRequest>
    {
        public DoctorDashboardSpec(Guid doctorId)
            : base(cr => cr.DoctorId == doctorId && cr.Status == RequestStatus.Pending)
        {
            AddInclude(cr => cr.PatientCase);
            AddInclude(cr => cr.Student);
        }
    }
}