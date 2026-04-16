using DentalHub.Application.Interfaces;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Repository;
using MediatR;

namespace DentalHub.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
      
        IMainRepository<T> GetRepository<T>() where T : class;
		IMainRepository<User> Users { get; }
        IMainRepository<Student> Students { get; }
        IPatientRepository Patients { get; }
        IMainRepository<Doctor> Doctors { get; }
        IMainRepository<Admin> Admins { get; }
        IPatientCaseRepository PatientCases { get; }
        ICaseRequestRepository CaseRequests { get; }
        IMainRepository<Session> Sessions { get; }
        IMainRepository<SessionNote> SessionNotes { get; }
        IMainRepository<Media> Medias { get; }
        IMainRepository<UniversityMember> UniversityMembers { get; }
        IMainRepository<CaseType> CaseTypes { get; }
        IMainRepository<Diagnosis> Diagnoses { get; }
        IMainRepository<University> Universities { get; }

       
        Task<int> SaveChangesAsync();
        int SaveChanges();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
