using FluentValidation;
using FluentValidation.Resources;
using MyBuyingList.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(opt =>
{
    opt.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
    opt.IncludeScopes = false;
});
builder.Logging.AddSeq(builder.Configuration.GetSection("Seq"));

string secretsLocation = builder.Configuration["SecretsLocation"]
    ?? throw new InvalidOperationException("SecretsLocation not found in configuration.");

if (builder.Environment.EnvironmentName is ("Test" or "Development"))
{
    secretsLocation = Path.Combine(Directory.GetCurrentDirectory(), secretsLocation);
}

builder.Configuration.AddKeyPerFile(directoryPath: secretsLocation, optional: false, reloadOnChange: true);

builder.Services.AddServices(GetLogger(builder.Configuration), builder.Configuration);

var app = builder.Build();
await app.StartApplication();
app.Logger.LogInformation("Running app...");

ValidatorOptions.Global.LanguageManager = new LanguageManager()
{
    Culture = System.Globalization.CultureInfo.GetCultureInfo("en"),
};

app.Run();

ILogger<Program> GetLogger(IConfiguration configuration)
{
    // 1. Manually create a logger factory
    using var loggerFactory = LoggerFactory.Create(loggingBuilder => 
    {
        loggingBuilder.AddConsole();
        loggingBuilder.AddSeq(configuration.GetSection("Seq"));
    });

// 2. Create a logger instance
    ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
    return logger;
}