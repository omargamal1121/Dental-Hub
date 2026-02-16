using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.CaseRequests
{
    public class CaseRequestWithDetailsSpec : BaseSpecification<CaseRequest>
    {
        public CaseRequestWithDetailsSpec(Guid requestId)
            : base(cr => cr.Id == requestId)
        {
            AddInclude(cr => cr.PatientCase);
            AddInclude(cr => cr.Student);
            AddInclude(cr => cr.Doctor);
        }
    }
}
