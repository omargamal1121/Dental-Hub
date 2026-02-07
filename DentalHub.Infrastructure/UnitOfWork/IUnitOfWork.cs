using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Repository;

namespace DentalHub.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IMainRepository<User> Users { get; }
        IMainRepository<Student> Students { get; }
        IMainRepository<Patient> Patients { get; }
        IMainRepository<Doctor> Doctors { get; }
        IMainRepository<Admin> Admins { get; }
        IMainRepository<PatientCase> PatientCases { get; }
        IMainRepository<CaseRequest> CaseRequests { get; }
        IMainRepository<Session> Sessions { get; }
        IMainRepository<SessionNote> SessionNotes { get; }
        IMainRepository<Media> Medias { get; }

        // Save Changes
        Task<int> SaveChangesAsync();
        int SaveChanges();

        // Transactions
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
