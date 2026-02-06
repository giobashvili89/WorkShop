namespace WorkShop.Application.Models;

public class BookRequestModel
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
}
