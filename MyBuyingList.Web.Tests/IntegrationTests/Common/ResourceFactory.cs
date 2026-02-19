using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Infrastructure;
using System.Net.Http.Headers;
using MyBuyingList.Domain.Constants;
using Testcontainers.PostgreSql;

namespace MyBuyingList.Web.Tests.IntegrationTests.Common;

// Video I got inspired by: https://www.youtube.com/watch?v=E4TeWBFzcCw
public class ResourceFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly int _exposedPort = new Random().Next(1000, 10000);

    public IConfiguration Configuration { get; private set; } = null!;
    public HttpClient HttpClient { get; private set; } = null!;

    public ResourceFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14")
            .WithUsername("myuser")
            .WithPassword("password")
            .WithDatabase("db")
            .WithHostname("localhost")
            .WithPortBinding(_exposedPort, 5432)  
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        
        var manager = new ConfigurationManager();
        manager
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.IntegrationTests.json"), optional: false)
            .Build();

        builder.UseConfiguration(manager);
        Configuration = manager;
        
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

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Creates client and adds JWT so it doesnt need to authenticate
        // on every test.

        var client = CreateClient();

        LoginRequest loginDto = new()
        {
            Username = Users.AdminUser.UserName,
            Password = Users.AdminUser.Password
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
        using var scope = Services.CreateScope();
        var dbService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbService.Database.EnsureDeletedAsync();

        await dbService.Database.EnsureCreatedAsync();
    }
}
