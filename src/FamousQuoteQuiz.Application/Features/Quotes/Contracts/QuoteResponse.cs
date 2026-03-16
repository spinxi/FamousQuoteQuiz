namespace FamousQuoteQuiz.Application.Features.Quotes.Contracts;

public sealed class QuoteResponse
{
    public int Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
