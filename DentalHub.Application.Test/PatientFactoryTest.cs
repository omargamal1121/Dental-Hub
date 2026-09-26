using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Entities;
using DentalHub.Domain.Factories;

namespace DentalHub.Application.Test;

public class PatientFactoryTest
{


	[Theory]
	[InlineData("00000000-0000-0000-0000-000000000000", 22, "01226493558", "UserId cannot be empty")]
	[InlineData("11111111-1111-1111-1111-111111111111", 0, "01226493558", "Age must be greater than 0")]
	[InlineData("11111111-1111-1111-1111-111111111111", 22, "", "Phone cannot be empty")]
	[InlineData("11111111-1111-1111-1111-111111111111", 22, "abc123", "Phone format is invalid")]
	public void Create_InvalidInput_ThrowsDomainExceptionWithCorrectMessage(
		string userIdStr, int age, string phone, string expectedMessage)
	{
		var userId = Guid.Parse(userIdStr);

		var ex = Assert.Throws<DomainException>(() =>
			PatientFactory.Create(userId, age, phone));

		Assert.Equal(expectedMessage, ex.Message);
	}
	[Fact]
	public void Create_ValidData_ReturnsPatientWithCorrectValues()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var age = 22;
		var phone = "01226493558";

		// Act
		var result = PatientFactory.Create(userId, age, phone);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(age, result.Age);
		Assert.Equal(phone, result.Phone);
		Assert.True((DateTime.UtcNow - result.CreateAt).TotalSeconds < 5);
	}

}
