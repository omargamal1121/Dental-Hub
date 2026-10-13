using DentalHub.Application.Services;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace DentalHub.Application.Test;

public class PatientServiceTests
{
	private readonly PatientService _patientService;

	private static Mock<UserManager<User>> CreateMockUserManager()
	{
		var store = new Mock<IUserStore<User>>();
		return new Mock<UserManager<User>>(
			store.Object, null, null, null, null, null, null, null, null);
	}
	public PatientServiceTests()
	{
		var iuwMock = new Mock<IUnitOfWork>();

		iuwMock.Setup(p => p.Patients.GetByIdAsync(It.IsAny<Guid>()))
			   .ReturnsAsync(new Patient { });

		_patientService=new PatientService(iuwMock.Object,(new Mock<ILogger<PatientService>>()).Object, CreateMockUserManager().Object);

	}
	[Fact]
	public async Task GetPatientByIdAsync_ValidUser_RetrunPatient()
	{
		var id=Guid.NewGuid();
		var patient = await _patientService.GetPatientByIdAsync(id);
		Assert.NotNull(patient);
	}


}
