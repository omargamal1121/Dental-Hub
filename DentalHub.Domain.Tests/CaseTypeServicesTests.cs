using Castle.Core.Logging;
using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Application.Services.CaseTypes;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;

namespace DentalHub.Domain.Tests;

public class CaseTypeServicesTests
{
	[Fact]
	public async Task UpdateCaseTypeAsync_CaseTypeNotFound_ReturnFailure()
	{
		//arrage
		var data = new UpdateCaseTypeDto
		{
			Id = Guid.NewGuid(),

		};
		var mockUnitOfWork = new Mock<IUnitOfWork>();
		mockUnitOfWork.Setup( u => u.CaseTypes.GetByIdAsync(It.IsAny<ISpecification<CaseType>>()))
			.ReturnsAsync((CaseType)null);
		;
		var CaseTypeService = new CaseTypeService(mockUnitOfWork.Object, new Mock<ILogger<CaseTypeService>>().Object);
		var result= await CaseTypeService.UpdateCaseTypeAsync(data);

		Assert.False(result.IsSuccess);
	}

}
