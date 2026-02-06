using Microsoft.EntityFrameworkCore;
using WorkShop.Domain.Entities;
using WorkShop.Infrastructure.Services;

namespace WorkShop.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await context.Books.AnyAsync() || await context.Users.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed Books - 100 records
        await SeedBooksAsync(context);

        // Seed Default Admin User
        await SeedDefaultUserAsync(context);

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
        var random = new Random(42); // Fixed seed for reproducibility

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
                PublishedDate = DateTime.SpecifyKind(new DateTime(publishedYear, publishedMonth, publishedDay), DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.Books.AddRangeAsync(books);
    }

    private static async Task SeedDefaultUserAsync(AppDbContext context)
    {
        // Create default admin user
        // Note: Password validation requires minimum 6 characters, using "admin1" instead of "admin"
        var passwordHash = PasswordHasher.HashPassword("admin1");

        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@workshop.com",
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(adminUser);
    }
}
