using CatalogService.Api.Extensions;
using CatalogService.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.ConfigureConsul(builder.Configuration);

var app = builder.Build();

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
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(System.IO.Path.Combine(app.Environment.ContentRootPath, "Pics")),
    RequestPath = "/pics"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Register with Consul (if applicable in your project)
app.RegisterWithConsul(app.Lifetime, builder.Configuration);

app.Run();
