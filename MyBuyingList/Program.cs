using MyBuyingList.Web;

var builder = WebApplication.CreateBuilder(args);

// tried to use LoggerFactory, but it did not work properly.
// Will try another way for logging the startup.
var logger = builder.Services.BuildServiceProvider().GetService<ILogger<Program>>()!;
var isDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddServices(logger, builder.Configuration, isDevelopment);

var app = builder.Build();
await app.StartApplication();
logger.LogInformation("Running app...");
app.Run();