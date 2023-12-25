using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyBuyingList.Infrastructure;
using MyBuyingList.Web.Middlewares.RateLimiting;
using System.Net.Http.Headers;
using Testcontainers.PostgreSql;

namespace MyBuyingList.Web.Tests.IntegrationTests.Common;

// Video I got inspired by: https://www.youtube.com/watch?v=E4TeWBFzcCw
public class ResourceFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private int ExposedPort = new Random().Next(1000, 10000);

    public HttpClient HttpClient { get; private set; } = default!;

    public ResourceFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithUsername("myuser")
            .WithPassword("password")
            .WithDatabase("db")
            .WithHostname("localhost")
            .WithPortBinding(ExposedPort, 5432)
            // TODO: wait until can create a new connection. Until port is available is not working.
            //.WithWaitStrategy(Wait.ForWindowsContainer().UntilPortIsAvailable(RandomPort))             
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove AppDbContext
            var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            services.Remove(descriptor);

            // Add them changing the connection string
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention();
            });
            //services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddScoped<ApplicationDbContext>();
            
            services.Configure<CustomRateLimiterOptions>(opts =>
            {
                opts.PermitLimit = 1_000_000;
                opts.QueueLimit = 0;
                opts.Window = 5;
            });
        });

    }

    // TODO: change JWT expiring time to 10 minutes so tests does not expire.
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Creates client and adds JWT so it doesnt need to authenticate
        // on every test.

        var client = CreateClient();

        var response = await client.GetAsync("api/auth?username=admin&password=123");
        var token = await response.Content.ReadAsStringAsync();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpClient = client;
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using (var scope = Services.CreateScope())
        {
            var dbService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbService is not null)
            {
                await dbService.Database.EnsureDeletedAsync();

                await dbService.Database.EnsureCreatedAsync();
            }
        }
    }
}
