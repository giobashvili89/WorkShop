using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Enums;
using WorkShop.Infrastructure.Services;

namespace WorkShop.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            // Ensure database exists and apply all pending migrations
            // MigrateAsync() handles both creating the database (if needed) and applying migrations
            logger.LogInformation("Ensuring database exists and applying migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully.");

            // Check if data already exists (after migrations are applied)
            if (await context.Categories.AnyAsync())
            {
                logger.LogInformation("Database already contains data. Skipping seeding.");
                return;  
            }

            logger.LogInformation("Database is empty. Starting seeding process...");

            // Seed Categories first
            await SeedCategoriesAsync(context, logger);
            
            // Seed Books - 100 records
            await SeedBooksAsync(context, logger);

            // Seed Default Users (Admin and Customer)
            await SeedDefaultUsersAsync(context, logger);

            await context.SaveChangesAsync();
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during database initialization.");
            throw;
        }
    }

    private static async Task SeedCategoriesAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding categories...");
        var categories = new[]
        {
            new Category { Name = "Fiction", Description = "Fictional literature including novels and short stories" },
            new Category { Name = "Non-Fiction", Description = "Factual books including biographies, history, and essays" },
            new Category { Name = "Science", Description = "Scientific books covering various scientific disciplines" },
            new Category { Name = "History", Description = "Historical books and documentation" },
            new Category { Name = "Biography", Description = "Life stories and biographies of notable people" },
            new Category { Name = "Technology", Description = "Books about technology, programming, and computing" },
            new Category { Name = "Art", Description = "Books about art, design, and creativity" },
            new Category { Name = "Philosophy", Description = "Philosophical texts and discussions" },
            new Category { Name = "Self-Help", Description = "Personal development and self-improvement books" },
            new Category { Name = "Mystery", Description = "Mystery and detective fiction" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} categories.", categories.Length);
    }

    private static async Task SeedBooksAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding books...");
        // Get all categories to use their IDs
        var categories = await context.Categories.ToListAsync();
        var categoryDict = categories.ToDictionary(c => c.Name, c => c.Id);
        
        var authors = new[]
        {
            "George Orwell", "Jane Austen", "Mark Twain", "Charles Dickens", "Leo Tolstoy",
            "F. Scott Fitzgerald", "Ernest Hemingway", "William Shakespeare", "J.K. Rowling", "Stephen King",
            "Agatha Christie", "Arthur Conan Doyle", "J.R.R. Tolkien", "Gabriel García Márquez", "Virginia Woolf",
            "James Joyce", "Franz Kafka", "Albert Camus", "Hermann Hesse", "Oscar Wilde"
        };

        var books = new List<Book>();
        var random = new Random(42); 

        for (int i = 1; i <= 100; i++)
        {
            var author = authors[random.Next(authors.Length)];
            var category = categories[random.Next(categories.Count)];
            var publishedYear = random.Next(1900, 2024);
            var publishedMonth = random.Next(1, 13);
            var daysInMonth = DateTime.DaysInMonth(publishedYear, publishedMonth);
            var publishedDay = random.Next(1, daysInMonth + 1);

            books.Add(new Book
            {
                Title = $"{category.Name} Book {i}",
                Author = author,
                CategoryId = category.Id,
                Description = $"This is a comprehensive {category.Name.ToLower()} book written by {author}. " +
                             $"It covers various aspects of {category.Name.ToLower()} and provides valuable insights. " +
                             $"Published in {publishedYear}, this work has been influential in its field.",
                ISBN = $"978-{random.Next(1000000000, 2000000000)}", // Mock ISBN for demo purposes
                Price = Math.Round((decimal)(random.NextDouble() * 50 + 9.99), 2), // Price between $9.99 and $59.99
                StockQuantity = random.Next(10, 100), // Stock between 10 and 100
                SoldCount = 0,
                PublishedDate = DateTime.SpecifyKind(new DateTime(publishedYear, publishedMonth, publishedDay), DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.Books.AddRangeAsync(books);
        logger.LogInformation("Seeded {Count} books.", books.Count);
    }

    private static async Task SeedDefaultUsersAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding default users...");
        // Create default admin user
        // Note: Password validation requires minimum 6 characters, using "admin1" instead of "admin"
        var adminPasswordHash = PasswordHasher.HashPassword("admin1");

        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@workshop.com",
            PasswordHash = adminPasswordHash,
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(adminUser);

        // Create default customer user for testing
        var customerPasswordHash = PasswordHasher.HashPassword("customer1");

        var customerUser = new User
        {
            Username = "customer",
            Email = "customer@workshop.com",
            PasswordHash = customerPasswordHash,
            Role = UserRole.Customer,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(customerUser);
        
        logger.LogInformation("Seeded 2 default users (admin and customer).");
    }
}
