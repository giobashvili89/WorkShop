using Microsoft.EntityFrameworkCore;
using WorkShop.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

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

            books.Add(new Book
            {
                Title = $"{category} Book {i}",
                Author = author,
                Category = category,
                Description = $"This is a comprehensive {category.ToLower()} book written by {author}. " +
                             $"It covers various aspects of {category.ToLower()} and provides valuable insights. " +
                             $"Published in {publishedYear}, this work has been influential in its field.",
                PublishedDate = new DateTime(publishedYear, random.Next(1, 13), random.Next(1, 29)),
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.Books.AddRangeAsync(books);
    }

    private static async Task SeedDefaultUserAsync(AppDbContext context)
    {
        // Create default admin user with username: admin, password: admin
        var passwordHash = HashPassword("admin");

        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@workshop.com",
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(adminUser);
    }

    private static string HashPassword(string password)
    {
        // Use the same hashing algorithm as AuthService
        const int SaltSize = 16;
        const int HashSize = 32;
        const int Iterations = 100000;

        // Generate a random salt
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        // Hash password with PBKDF2
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        // Combine salt and hash
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // Convert to base64 for storage
        return Convert.ToBase64String(hashBytes);
    }
}
