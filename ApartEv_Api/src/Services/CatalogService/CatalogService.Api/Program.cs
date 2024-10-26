using CatalogService.Api.Extensions;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Set up the configuration for appsettings and Serilog files
var env = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Configurations/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Configurations/appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var serilogConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Configurations/serilog.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Configurations/serilog.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(serilogConfiguration)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger, true);

// Add services to the container.
builder.Services.AddControllers();

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CatalogService.Api", Version = "v1" });
});

// Configuration for CatalogSettings
builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection("CatalogSettings"));

// Configure the DbContext
builder.Services.ConfigureDbContext(builder.Configuration);

// Configure Consul registration (if applicable in your project)
// TODO BU SATIRI AÇ ÝLERÝDE builder.Services.ConfigureConsul(builder.Configuration);

var app = builder.Build();

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CatalogContext>();
    var envService = services.GetRequiredService<IWebHostEnvironment>();
    var logger = services.GetRequiredService<ILogger<CatalogContextSeed>>();

    new CatalogContextSeed()
        .SeedAsync(context, envService, logger)
        .Wait();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogService.Api v1"));
}

app.UseHttpsRedirection();

// Configure static file serving with custom folder path
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "Pics")),
    RequestPath = "/pics"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Register with Consul (if applicable in your project)
// TODO BU SATIRI AÇ ÝLERÝDE app.RegisterWithConsul(app.Lifetime, builder.Configuration);

try
{
    Log.Logger.Information("Application is Running....");
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
