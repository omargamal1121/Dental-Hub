using DentalHub.Infrastructure.ContextAndConfig;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Respawn;
using System.Data.Common;
using System.Data.Entity;
using Testcontainers.MySql;

namespace DentalHub.Application.IntegrationTests;

public class MySqlFixture : IAsyncLifetime
{

	public MySqlContainer Container { get; private set; } = null!;
	public Respawner Respawn { get; private set; } = null!;
	public ContextApp ContextApp { get; private set; } = null!;
	private MySqlConnection _MySqlConnection =null!;
	public  CusomeWebApHostFactory cusomeWebApHostFactory { get; private set; } = null!;
	public async Task DisposeAsync()
	{
		await _MySqlConnection.CloseAsync();
		await ContextApp.DisposeAsync();
		await Container.DisposeAsync();
	}

	public async Task InitializeAsync()
	{
		Container = new MySqlBuilder("mysql:8.0.34")
			.WithDatabase("testdb")
			.WithUsername("testuser")
			.WithPassword("password")
			.Build();
		await Container.StartAsync();

		var options = new DbContextOptionsBuilder<ContextApp>()
		.UseMySql(
			Container.GetConnectionString(),
			ServerVersion.AutoDetect(Container.GetConnectionString()))
		.Options;




		ContextApp = new ContextApp(options);
		 await	 ContextApp.Database.MigrateAsync();
		_MySqlConnection = new MySqlConnection(Container.GetConnectionString());
		await _MySqlConnection.OpenAsync();
		await UserMangerFactory.SeedRolesAsync(ContextApp);
		cusomeWebApHostFactory=new CusomeWebApHostFactory(Container.GetConnectionString());

		Respawn = await Respawner.CreateAsync(_MySqlConnection, new RespawnerOptions
		{
			DbAdapter = DbAdapter.MySql,
			TablesToIgnore = ["AspNetRoles", "AspNetUserRoles"]
		});
	}
	public async Task ResetDatabaseAsync()
	{
		await Respawn.ResetAsync(_MySqlConnection);
	}
}
