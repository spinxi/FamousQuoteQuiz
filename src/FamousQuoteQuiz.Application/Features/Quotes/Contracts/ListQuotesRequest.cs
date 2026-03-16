namespace FamousQuoteQuiz.Application.Features.Quotes.Contracts;

public sealed class ListQuotesRequest
{
    public string? Search { get; set; }
    public string? AuthorName { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
