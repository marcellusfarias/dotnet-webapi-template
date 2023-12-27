using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Infrastructure;
using MyBuyingList.Infrastructure.Authentication.JwtSetup;
using MyBuyingList.Infrastructure.Repositories;
using MyBuyingList.Web.Middlewares.RateLimiting;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Testcontainers.PostgreSql;

namespace MyBuyingList.Web.Tests.IntegrationTests.Common;

// Video I got inspired by: https://www.youtube.com/watch?v=E4TeWBFzcCw
public class ResourceFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private int ExposedPort = new Random().Next(1000, 10000);

    public IConfiguration Configuration { get; private set; } = default!;
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
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, conf) =>
        {
            // expand default config with settings designed for Integration Tests
            conf.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.IntegrationTests.json"));
            conf.AddEnvironmentVariables();

            // here we can "compile" the settings. Api.Setup will do the same, it doesn't matter.
            Configuration = conf.Build();
        });

        builder.ConfigureTestServices(services =>
        {
            // Replace with proper DbContext
            var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention();
            });
            services.AddScoped<ApplicationDbContext>();
        });

    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Creates client and adds JWT so it doesnt need to authenticate
        // on every test.

        var client = CreateClient();

        var loginDto = new LoginDto
        {
            Username = "admin",
            Password = "123"
        };

        var response = await client.PostAsync("api/auth", Utils.GetJsonContentFromObject(loginDto));
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
