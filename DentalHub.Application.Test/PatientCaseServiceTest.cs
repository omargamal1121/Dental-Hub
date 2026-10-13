using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Interfaces;
using DentalHub.Application.Services.Cases;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace DentalHub.Application.Test;

public class PatientCaseServiceTest
{
	
	public PatientCaseServiceTest()
	{
	

		
	}
	[Fact]
	public async Task GetAllCasesAsync_WhenSuccessful_ReturnsSuccessResult()
	{
		// Arrange

		var mockUow = new Mock<IUnitOfWork>();
		var id = Guid.NewGuid();
		var fakeCases = new List<PatientCaseDto> { new PatientCaseDto { Id = id } };



		mockUow.Setup(u => u.PatientCases.GetAllAsync<PatientCaseDto>(It.IsAny<ISpecificationWithProjection<PatientCase, PatientCaseDto>>()))
			   .ReturnsAsync(fakeCases);
		mockUow.Setup(u => u.PatientCases.CountAsync(It.IsAny<ISpecification<PatientCase>>()))
			   .ReturnsAsync(1);

		
		;
		var sut = new PatientCaseService(mockUow.Object, new Mock<ILogger<PatientCaseService>>().Object, new Mock<IMediaService>().Object);
		// Act
		var result = await sut.GetAllCasesAsync(page: 1, pageSize: 10);

		// Assert
		mockUow.Verify(u => u.PatientCases.GetAllAsync<PatientCaseDto>(
	It.IsAny<BaseSpecificationWithProjection<PatientCase, PatientCaseDto>>()), Times.Once);

		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Data);
		Assert.Single(result.Data.Items);
		Assert.Contains(result.Data.Items, p => p.Id == id);
	}

	[Fact]
	public async Task GetAllCasesAsync_WhenRepositoryThrows_ReturnsFailureResult()
	{
		// Arrange
		var mockUow = new Mock<IUnitOfWork>();
		mockUow.Setup(u => u.PatientCases.GetAllAsync(It.IsAny<ISpecificationWithProjection<PatientCase, PatientCaseDto>>()))
			   .ThrowsAsync(new Exception("DB connection failed"));
		mockUow.Verify(u=>u.SaveChangesAsync(), Times.Never);

	 var	sut = new PatientCaseService(mockUow.Object, new Mock<ILogger<PatientCaseService>>().Object, new Mock<IMediaService>().Object);



		var result = await sut.GetAllCasesAsync(page: 1, pageSize: 10);

		// Assert
		Assert.False(result.IsSuccess);
	
	}
	private static List<string> InvokeComputeAvailableActions(CaseUserFlags flags, PatientCaseDto dto)
	{
		var method = typeof(PatientCaseService).GetMethod(
			"ComputeAvailableActions",
			BindingFlags.NonPublic | BindingFlags.Static);

		return (List<string>)method!.Invoke(null, new object[] { flags, dto })!;
	}

	[Fact]
	public void Patient_Owner_PendingCase_ReturnsRemoveAndViewActions()
	{
		var flags = new CaseUserFlags { Role = "Patient", IsOwner = true };
		var dto = new PatientCaseDto { Status = CaseStatus.Pending.ToString() };

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().Contain("Remove");
		actions.Should().Contain("ViewSessions");
		actions.Should().Contain("ViewDiagnosis");
	}

	[Fact]
	public void Patient_Owner_NonPendingCase_DoesNotReturnRemove()
	{
		var flags = new CaseUserFlags { Role = "Patient", IsOwner = true };
		var dto = new PatientCaseDto { Status = CaseStatus.InProgress.ToString() };

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().NotContain("Remove");
		actions.Should().Contain("ViewSessions");
	}

	[Fact]
	public void Patient_NotOwner_ReturnsNoActions()
	{
		var flags = new CaseUserFlags { Role = "Patient", IsOwner = false };
		var dto = new PatientCaseDto { Status = CaseStatus.Pending.ToString() };

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().BeEmpty();
	}

	[Fact]
	public void Doctor_AssignedToMe_InProgress_ReturnsCompleteCase()
	{
		var flags = new CaseUserFlags { Role = "Doctor", IsAssignedToMe = true };
		var dto = new PatientCaseDto { Status = CaseStatus.InProgress.ToString() };

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().Contain("CompleteCase");
		actions.Should().Contain("EvaluateSession");
		actions.Should().Contain("AddDiagnosis");
	}

	[Fact]
	public void Doctor_AssignedToMe_NotInProgress_DoesNotReturnCompleteCase()
	{
		var flags = new CaseUserFlags { Role = "Doctor", IsAssignedToMe = true };
		var dto = new PatientCaseDto { Status = CaseStatus.Pending.ToString() };

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().NotContain("CompleteCase");
	}

	[Fact]
	public void Doctor_HasPendingRequest_NotAssignedYet_ReturnsAcceptAndReject()
	{
		var flags = new CaseUserFlags
		{
			Role = "Doctor",
			IsAssignedToMe = false,
			HasRequest = true,
			RequestStatus = RequestStatus.Pending.ToString()
		};
		var dto = new PatientCaseDto { Status = CaseStatus.Pending.ToString(), AssignedStudentId = null };

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().Contain("AcceptRequest");
		actions.Should().Contain("RejectRequest");
	}

	[Fact]
	public void Doctor_HasPendingRequest_AlreadyAssigned_ReturnsRejectOnly()
	{
		var flags = new CaseUserFlags
		{
			Role = "Doctor",
			IsAssignedToMe = false,
			HasRequest = true,
			RequestStatus = RequestStatus.Pending.ToString()
		};
		var dto = new PatientCaseDto { Status = CaseStatus.InProgress.ToString(), AssignedStudentId = Guid.NewGuid() };

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().NotContain("AcceptRequest");
		actions.Should().Contain("RejectRequest");
	}

	[Fact]
	public void Student_AssignedToMe_ReturnsSessionActions()
	{
		var flags = new CaseUserFlags { Role = "Student", IsAssignedToMe = true };
		var dto = new PatientCaseDto();

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().Contain("CreateSession");
		actions.Should().Contain("AddSessionNote");
	}

	[Fact]
	public void Student_NotAssigned_PublicPendingCase_NoExistingRequest_ReturnsRequestCase()
	{
		var flags = new CaseUserFlags { Role = "Student", IsAssignedToMe = false, HasRequest = false };
		var dto = new PatientCaseDto { Status = CaseStatus.Pending.ToString(), IsPublic = true };

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().Contain("RequestCase");
	}

	[Fact]
	public void Student_HasPendingRequest_ReturnsCancelRequest()
	{
		var flags = new CaseUserFlags
		{
			Role = "Student",
			IsAssignedToMe = false,
			HasRequest = true,
			RequestStatus = RequestStatus.Pending.ToString()
		};
		var dto = new PatientCaseDto();

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().Contain("CancelRequest");
	}

	[Fact]
	public void UnknownRole_ReturnsNoActions()
	{
		var flags = new CaseUserFlags { Role = "Unknown" };
		var dto = new PatientCaseDto();

		var actions = InvokeComputeAvailableActions(flags, dto);

		actions.Should().BeEmpty();
	}
}
