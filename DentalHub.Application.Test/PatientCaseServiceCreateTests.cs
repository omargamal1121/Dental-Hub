using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Interfaces;
using DentalHub.Application.Services.Cases;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace DentalHub.Application.Test
{

	public class PatientCaseServiceCreateTests
	{
		private readonly Mock<IUnitOfWork> _mockUow;
		private readonly Mock<IMediaService> _mockMediaService;
		private readonly PatientCaseService _sut;

		public PatientCaseServiceCreateTests()
		{
			_mockUow = new Mock<IUnitOfWork>();
			_mockMediaService = new Mock<IMediaService>();
			_sut = new PatientCaseService(
				_mockUow.Object,
				new Mock<ILogger<PatientCaseService>>().Object,
				_mockMediaService.Object);
		}

		[Fact]
		public async Task CreateCaseAsync_PatientNotFound_ReturnsFailure()
		{
			// Arrange
			_mockUow.Setup(u => u.Patients.GetByIdAsync(It.IsAny<ISpecification<Patient>>()))
					.ReturnsAsync((Patient?)null);

			var dto = new CreateCaseDto { PatientId = Guid.NewGuid() };

			// Act
			var result = await _sut.CreateCaseAsync(dto);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Errors.Should().ContainSingle("Patient not found");

			_mockUow.Verify(u => u.PatientCases.AddAsync(It.IsAny<PatientCase>()), Times.Never);
			_mockUow.Verify(u => u.SaveChangesAsync(), Times.Never);
		}

		[Fact]
		public async Task CreateCaseAsync_AddAsyncThrows_ReturnsFailure()
		{
			// Arrange
			_mockUow.Setup(u => u.Patients.GetByIdAsync(It.IsAny<ISpecification<Patient>>()))
					.ReturnsAsync(new Patient { Id = Guid.NewGuid() });

			_mockUow.Setup(u => u.PatientCases.AddAsync(It.IsAny<PatientCase>()))
					.ThrowsAsync(new Exception("DB insert failed"));

			var dto = new CreateCaseDto { PatientId = Guid.NewGuid() };

			// Act
			var result = await _sut.CreateCaseAsync(dto);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Errors.Should().ContainSingle("DB insert failed");

		}

		[Fact]
		public async Task CreateCaseAsync_SaveChangesThrows_ReturnsFailure()
		{
			// Arrange
			_mockUow.Setup(u => u.Patients.GetByIdAsync(It.IsAny<ISpecification<Patient>>()))
					.ReturnsAsync(new Patient { Id = Guid.NewGuid() });

			_mockUow.Setup(u => u.PatientCases.AddAsync(It.IsAny<PatientCase>()))
					.ReturnsAsync((PatientCase pc) => pc);  // ✅ اتصلحت

			_mockUow.Setup(u => u.SaveChangesAsync())
					.ThrowsAsync(new Exception("DB save failed"));

			var dto = new CreateCaseDto { PatientId = Guid.NewGuid() };

			// Act
			var result = await _sut.CreateCaseAsync(dto);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Errors.Should().ContainSingle("Error creating case");
		}
		[Fact]

		public async Task CreateCaseAsync_ValidData_CreatesCaseAndReturnsSuccess()
		{
			// Arrange
			var patientId = Guid.NewGuid();
			var caseId = Guid.NewGuid();

			_mockUow.Setup(u => u.Patients.GetByIdAsync(It.IsAny<ISpecification<Patient>>()))
					.ReturnsAsync(new Patient { Id = patientId });

			_mockUow.Setup(u => u.PatientCases.AddAsync(It.IsAny<PatientCase>()))
					.Callback<PatientCase>(pc => pc.Id = caseId)
					.ReturnsAsync((PatientCase pc) => pc);  // ✅ اتصلحت هنا

			_mockUow.Setup(u => u.SaveChangesAsync())
					.ReturnsAsync(1);

			var expectedDto = new PatientCaseDto { Id = caseId };
			_mockUow.Setup(u => u.PatientCases.GetByIdAsync(
						It.IsAny<ISpecificationWithProjection<PatientCase, PatientCaseDto>>()))
					.ReturnsAsync(expectedDto);

			var dto = new CreateCaseDto { PatientId = patientId, Description = "Cavity" };

			// Act
			var result = await _sut.CreateCaseAsync(dto);

			// Assert
			result.IsSuccess.Should().BeTrue();
			_mockUow.Verify(u => u.PatientCases.AddAsync(
				It.Is<PatientCase>(pc => pc.PatientId == patientId)), Times.Once);
			_mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
		}
	}
}