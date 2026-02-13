using FluentValidation;
using FluentValidation.Resources;
using MyBuyingList.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(opt =>
{
    // Only config that I haven't found how to configure in appsettings.
    opt.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled; 
});

string secretsLocation = builder.Configuration["SecretsLocation"]
    ?? throw new InvalidOperationException("SecretsLocation not found in configuration.");

if (builder.Environment.EnvironmentName is "Test")
{
    secretsLocation = Path.Combine(Directory.GetCurrentDirectory(), secretsLocation);
}

builder.Configuration.AddKeyPerFile(directoryPath: secretsLocation, optional: false, reloadOnChange: true);

var logger = builder.Services.BuildServiceProvider().GetService<ILogger<Program>>()!; // TODO: fix warning in the future.
builder.Services.AddServices(logger, builder.Configuration);

var app = builder.Build();
await app.StartApplication();
logger.LogInformation("Running app...");

ValidatorOptions.Global.LanguageManager = new LanguageManager()
{
    Culture = System.Globalization.CultureInfo.GetCultureInfo("en"),
};

app.Run();

// This class exists for the Integration Tests.
// TODO: read and check if there is a better approach https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
public partial class Program { }