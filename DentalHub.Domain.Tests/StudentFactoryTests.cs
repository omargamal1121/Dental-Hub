using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Factories;

namespace DentalHub.Domain.Tests;

public class StudentFactoryTests
{
	[Theory]
	[InlineData("00000000-0000-0000-0000-000000000000", "", "00000000-0000-0000-0000-000000000000", "UserId cannot be empty")]
	[InlineData("11111111-1111-1111-1111-111111111111", "Banha", "00000000-0000-0000-0000-000000000000", "universityId cannot be empty")]
	public void StudentFactory_InvalidData_ThrowDomainException(Guid userId, string university, Guid universityId,string message)
	{

		var ex= Assert.Throws<DomainException>(() => StudentFactory.Create(userId, university, universityId));
		Assert.Equal(message, ex.Message);


	}
	[Fact]
	public void StudentFactory_ValidData_ReturnStudent()
	{
		// arrange 
		Guid userid = Guid.NewGuid();
		string university = "Banha";
		Guid universityid= Guid.NewGuid();
		//act
		var student = StudentFactory.Create(userid, university, universityid);
		//assert
		Assert.Equal(userid, student.Id);

	}

}
