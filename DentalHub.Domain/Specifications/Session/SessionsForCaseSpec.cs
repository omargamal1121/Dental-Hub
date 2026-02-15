using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.Sessions
{
    public class SessionsForCaseSpec : BaseSpecification<Session>
    {
        public SessionsForCaseSpec(Guid caseId)
            : base(s => s.CaseId == caseId)
        {
            AddInclude(s => s.Medias);
            AddInclude(s => s.SessionNotes);
        }
    }
}