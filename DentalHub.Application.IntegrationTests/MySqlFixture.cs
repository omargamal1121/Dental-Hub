using DentalHub.Infrastructure.ContextAndConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Respawn;
using Testcontainers.MySql;

namespace DentalHub.Application.IntegrationTests;

public class MySqlFixture : IAsyncLifetime
{
	public MySqlContainer Container { get; private set; } = null!;
	public Respawner Respawn { get; private set; } = null!;
	public CusomeWebApHostFactory cusomeWebApHostFactory { get; private set; } = null!;
	private MySqlConnection _MySqlConnection = null!;

	public async Task InitializeAsync()
	{
		Container = new MySqlBuilder("mysql:8.0.34")
			.WithDatabase("testdb")
			.WithUsername("testuser")
			.WithPassword("password")
			.Build();
		await Container.StartAsync();

		_MySqlConnection = new MySqlConnection(Container.GetConnectionString());
		await _MySqlConnection.OpenAsync();

		cusomeWebApHostFactory = new CusomeWebApHostFactory(Container.GetConnectionString());

		using (var scope = cusomeWebApHostFactory.Services.CreateScope())
		{
			//var context = scope.ServiceProvider.GetRequiredService<ContextApp>();
			//await context.Database.MigrateAsync();
			//await UserMangerFactory.SeedRolesAsync(context);
		}



		Respawn = await Respawner.CreateAsync(_MySqlConnection, new RespawnerOptions
		{
			DbAdapter = DbAdapter.MySql,
			TablesToIgnore = ["AspNetRoles", "AspNetUserRoles"]
		});
	}

	public IServiceScope CreateScope() => cusomeWebApHostFactory.Services.CreateScope();

	public async Task ResetDatabaseAsync() => await Respawn.ResetAsync(_MySqlConnection);

	public async Task DisposeAsync()
	{
		await _MySqlConnection.CloseAsync();
		await cusomeWebApHostFactory.DisposeAsync();
		await Container.DisposeAsync();
	}
}

