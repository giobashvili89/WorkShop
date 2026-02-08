using FluentValidation.AspNetCore;
using WorkShop.API.Configuration;
using WorkShop.API.Middleware;
using WorkShop.Application;
using WorkShop.Infrastructure;
using WorkShop.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Add FluentValidation Auto-Validation
builder.Services.AddFluentValidationAutoValidation();

// Add Application Services (FluentValidation Validators and MediatR)
builder.Services.AddApplicationServices();

// Add Swagger Configuration
builder.Services.AddSwaggerConfiguration();

// Add CORS Configuration
builder.Services.AddCorsConfiguration();

// Add Infrastructure Services (Database and Services)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting database initialization...");
        var context = services.GetRequiredService<AppDbContext>();
        await DbInitializer.InitializeAsync(context, logger);
        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
        throw; // Rethrow to prevent app from starting with broken database
    }
}

// Configure the HTTP request pipeline.
// Add global exception handler middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable static files for uploads
app.UseStaticFiles();

app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make the implicit Program class public and partial for testing
public partial class Program { }
