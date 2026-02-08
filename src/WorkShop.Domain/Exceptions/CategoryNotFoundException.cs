namespace WorkShop.Domain.Exceptions;

/// <summary>
/// Exception thrown when a category is not found
/// </summary>
public class CategoryNotFoundException : NotFoundException
{
    public CategoryNotFoundException(int categoryId) 
        : base($"Category with ID {categoryId} was not found.")
    {
    }

    public CategoryNotFoundException(string categoryName) 
        : base($"Category '{categoryName}' was not found.")
    {
    }
}
