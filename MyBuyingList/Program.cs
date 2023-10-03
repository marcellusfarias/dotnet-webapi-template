using MyBuyingList.Web;

var builder = WebApplication.CreateBuilder(args);

var logFactory = new LoggerFactory();
var logger = logFactory.CreateLogger<Program>();
builder.Services.AddServices(logger, builder.Configuration);

var app = builder.Build();

app.StartApplication(logger);
logger.LogInformation("Running app...");
app.Run();