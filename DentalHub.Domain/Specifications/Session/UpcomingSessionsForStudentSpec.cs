using DentalHub.Domain.Entities;
using DentalHub.Domain.Specifications;

public class UpcomingSessionsForStudentSpec : BaseSpecification<Session>
{
    public UpcomingSessionsForStudentSpec(Guid studentId)
        : base(s => s.StudentId == studentId && s.ScheduledAt > DateTime.UtcNow)
    {
        AddInclude(s => s.PatientCase);
        AddInclude(s => s.Doctor);
        AddInclude(s => s.Patient);
    }
}