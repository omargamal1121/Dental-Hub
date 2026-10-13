using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.ContextAndConfig;
using DentalHub.Infrastructure.Repository;
using DentalHub.Infrastructure.Repository.PatientCaseRepo;
using DentalHub.Infrastructure.Repository.PatientRepo;
using DentalHub.Infrastructure.Repository.RequestRepo;
using DentalHub.Infrastructure.UnitOfWork;

namespace DentalHub.Application.IntegrationTests;

public static class UnitOfWorkTestFactory
{
	public static UnitOfWork Create(ContextApp context)
	{
		return new UnitOfWork(
			serviceProvider: null!,
			users: new MainRepository<User>(context),
			students: new MainRepository<Student>(context),
			patients: new PatientRepository(context),
			doctors: new MainRepository<Doctor>(context),
			admins: new MainRepository<Admin>(context),
			patientCases: new PatientCaseRepository(context),
			caseRequests: new CaseRequestRepository(context),
			sessions: new MainRepository<Session>(context),
			sessionNotes: new MainRepository<SessionNote>(context),
			medias: new MainRepository<Media>(context),
			universityMembers: new MainRepository<UniversityMember>(context),
			caseTypes: new MainRepository<CaseType>(context),
			diagnoses: new MainRepository<Diagnosis>(context),
			universities: new MainRepository<University>(context),
			refreshTokens: new MainRepository<RefreshToken>(context),
			context: context,
			logger: null);
	}
}
