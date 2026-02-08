using Microsoft.EntityFrameworkCore;
using WorkShop.Domain.Entities;
using WorkShop.Domain.Enums;
using WorkShop.Infrastructure.Services;

namespace WorkShop.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Apply any pending migrations
        await context.Database.MigrateAsync();

        // Check if data already exists (after migrations are applied)
        if (await context.Books.AnyAsync())
        {
            return;  
        }

        // Seed Books - 100 records
        await SeedBooksAsync(context);

        // Seed Default Users (Admin and Customer)
        await SeedDefaultUsersAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedBooksAsync(AppDbContext context)
    {
        var categories = new[] { "Fiction", "Non-Fiction", "Science", "History", "Biography", "Technology", "Art", "Philosophy", "Self-Help", "Mystery" };
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
            var category = categories[random.Next(categories.Length)];
            var publishedYear = random.Next(1900, 2024);
            var publishedMonth = random.Next(1, 13);
            var daysInMonth = DateTime.DaysInMonth(publishedYear, publishedMonth);
            var publishedDay = random.Next(1, daysInMonth + 1);

            books.Add(new Book
            {
                Title = $"{category} Book {i}",
                Author = author,
                Category = category,
                Description = $"This is a comprehensive {category.ToLower()} book written by {author}. " +
                             $"It covers various aspects of {category.ToLower()} and provides valuable insights. " +
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
    }

    private static async Task SeedDefaultUsersAsync(AppDbContext context)
    {
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
    }
}
