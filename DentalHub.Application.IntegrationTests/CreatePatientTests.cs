using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Services;
using DentalHub.Infrastructure.ContextAndConfig;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;


namespace DentalHub.Application.IntegrationTests;

public class CreatePatientTests : IClassFixture<MySqlFixture> , IAsyncLifetime
{
	private readonly MySqlFixture _fixture;
	private  PatientService _patientService=null!;

	public CreatePatientTests(MySqlFixture fixture)
	{
		_fixture = fixture;

	
	}
	[Fact]
	public async Task CreatePatient_HappyCase_Success()
	{
		var client = _fixture.cusomeWebApHostFactory.CreateClient();
		var request = new CreatePatientCommand("Omar Gamal", "Admin@123", "01226493558"
			, "29403120103040",new DateTime(2004,3,12),Gender.Male,City.Cairo);
	
		var response = await client.PostAsJsonAsync("/api/v1/Patients", request);

		// اطبع الـ status code والـ body عشان نشوف السبب الحقيقي
		var responseBody = await response.Content.ReadAsStringAsync();

		response.IsSuccessStatusCode.Should().BeTrue(
			$"Expected success but got {response.StatusCode}. Response: {responseBody}");
	}
	[Fact]
	public async Task GetPatientByNationalId_HappyCase_success()
	{
		 await	 _fixture.ResetDatabaseAsync();
		// Arrange
		var data = new CreatePatientCommand(
			"Omar gamal", "Admin@123", "01226493558", "29403120103040",
			new DateTime(2004, 3, 12), Gender.Male, City.Suez);

		
		var Createresult = await _patientService.CreatePatientAsync(data);
		Createresult.IsSuccess.Should().BeTrue();
		Createresult.Data.Should().NotBeEmpty();
		
		var filterdata = new FilterPatientDto
		{
			NationalId = "29403120103040"
		};
		//act
		var result= await	_patientService.GetAllPatientsAsync(filterdata, 1, 10);

		//Assert
		result.IsSuccess.Should().BeTrue();
		result.Data.Should().NotBeNull();
		result.Data.Items.FirstOrDefault(p => p.NationalId == "29403120103040").Should().NotBeNull();
	}

	public Task DisposeAsync() => Task.CompletedTask;

	public async Task InitializeAsync()
	{
		var contextApp= _fixture.CreateScope().ServiceProvider.GetRequiredService<ContextApp>();
		await	contextApp.Database.MigrateAsync();
		var unitOfWork = UnitOfWorkTestFactory.Create(contextApp);
		var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
		var logger = loggerFactory.CreateLogger<PatientService>();
		var UserManager = UserMangerFactory.CreateRealUserManager(contextApp);

		_patientService = new PatientService(unitOfWork, logger, UserManager);
	}
}