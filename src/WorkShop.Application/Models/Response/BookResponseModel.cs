namespace WorkShop.Application.Models.Response;

public class BookResponseModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int SoldCount { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? CoverImagePath { get; set; }
}
