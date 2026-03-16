using FamousQuoteQuiz.Domain.Enums;

namespace FamousQuoteQuiz.Application.Features.Quiz.Contracts;

public sealed class SubmitAnswerRequest
{
    public int SessionId { get; set; }
    public int QuoteId { get; set; }
    public QuizMode Mode { get; set; }
    public string UserAnswer { get; set; } = string.Empty;
    public string? CandidateAuthor { get; set; }
    public IReadOnlyCollection<string> PresentedOptions { get; set; } = Array.Empty<string>();
}
