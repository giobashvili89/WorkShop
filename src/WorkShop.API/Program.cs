using FluentValidation.AspNetCore;
using WorkShop.API.Configuration;
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
    ILogger<Program>? logger = null;
    
    try
    {
        logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Starting database initialization...");
        
        var context = services.GetRequiredService<AppDbContext>();
        await DbInitializer.InitializeAsync(context);
        
        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        // Attempt to log error, but handle case where logger might not be available
        if (logger != null)
        {
            logger.LogCritical(ex, 
                "CRITICAL: Database initialization failed! " +
                "The application may not function correctly. " +
                "Common causes: " +
                "1. PostgreSQL is not running " +
                "2. Connection string is incorrect " +
                "3. Database permissions are insufficient " +
                "See DATABASE_TROUBLESHOOTING.md for detailed troubleshooting steps.");
        }
        else
        {
            // Fallback to Console if logger is not available
            Console.WriteLine($"CRITICAL: Database initialization failed: {ex.Message}");
            Console.WriteLine("See DATABASE_TROUBLESHOOTING.md for detailed troubleshooting steps.");
        }
    }
}

// Configure the HTTP request pipeline.
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
