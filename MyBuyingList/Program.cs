using MyBuyingList.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(opt =>
{
    // Only config that I haven't found how to configure in appsettings.
    opt.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled; 
});

var logger = builder.Services.BuildServiceProvider().GetService<ILogger<Program>>()!; // TODO: fix warning in the future.
var isDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddServices(logger, builder.Configuration, isDevelopment);

var app = builder.Build();
await app.StartApplication();
logger.LogInformation("Running app...");
app.Run();

// This class exists for the Integration Tests.
// TODO: read and check if there is a better approach https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
public partial class Program { }