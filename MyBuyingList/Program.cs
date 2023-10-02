using Microsoft.EntityFrameworkCore;
using MyBuyingList.Infrastructure;
using MyBuyingList.Application;
using MyBuyingList.Web.Middlewares;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.MiddlewareAnalysis;
using MyBuyingList.Web;
using System.Diagnostics;
using MyBuyingList.Web.Filters;

const bool _DEBUG = true;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Insert(0, ServiceDescriptor.Transient<IStartupFilter, AnalysisStartupFilter>());

ILogger<Program> logger = builder.Services.BuildServiceProvider().GetService<ILogger<Program>>()!;
builder.Services.AddInfrastructureServices(logger, builder.Configuration);
builder.Services.AddApplicationServices(logger);

// Add services to the container.
//builder.Services.AddControllersWithViews();

builder.Services.AddControllers(options => { options.Filters.Add(typeof(RequestBodyValidationFilter));});
builder.Services.AddLogging();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyBuyingList API",
        Version = "v1",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Here enter JWT Token with bearer format like bearer[space] token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

#region Adding debugging middleware logic

if(_DEBUG)
{
    // Grab the "Microsoft.AspNetCore" DiagnosticListener from DI
    var listener = app.Services.GetRequiredService<DiagnosticListener>();

    // Create an instance of the AnalysisDiagnosticAdapter using the IServiceProvider
    // so that the ILogger is injected from DI
    var observer = ActivatorUtilities.CreateInstance<AnalysisDiagnosticAdapter>(app.Services);

    // Subscribe to the listener with the SubscribeWithAdapter() extension method
    using var disposable = listener.SubscribeWithAdapter(observer);
}

#endregion

//needed for RequestBodyValidationFilter
app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{    
    app.UseExceptionHandler("/Home/Error");    
    app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}
else
{    
    app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
    var db = (ApplicationDbContext) scope.ServiceProvider.GetRequiredService(typeof(ApplicationDbContext));
    try
    {        
        db.Database.Migrate(); //Run migrations that havent been ran.
    }
    catch (Exception ex)
    {
        logger.LogError($"Failed to connect to database. Err: {ex.Message}, Exception: {ex.InnerException}");
    }    
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware(typeof(ErrorHandlingMiddleware));
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

logger.LogInformation("Running app");
app.Run();
