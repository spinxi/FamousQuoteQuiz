namespace FamousQuoteQuiz.Application.Features.Quotes.Contracts;

public sealed class ListQuotesRequest
{
    public string? Search { get; set; }
    public string? AuthorName { get; set; }
    public bool? IsActive { get; set; }
    public string SortBy { get; set; } = "authorName";
    public string SortDirection { get; set; } = "asc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
