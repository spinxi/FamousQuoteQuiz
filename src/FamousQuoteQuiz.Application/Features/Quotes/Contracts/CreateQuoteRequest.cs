namespace FamousQuoteQuiz.Application.Features.Quotes.Contracts;

public sealed class CreateQuoteRequest
{
    public string Text { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
