using FamousQuoteQuiz.Domain.Enums;

namespace FamousQuoteQuiz.Application.Features.Quiz.Contracts;

public sealed class QuizQuestionResponse
{
    public int SessionId { get; init; }
    public int QuoteId { get; init; }
    public string QuoteText { get; init; } = string.Empty;
    public QuizMode Mode { get; init; }
    public string? CandidateAuthor { get; init; }
    public IReadOnlyCollection<string> AnswerOptions { get; init; } = Array.Empty<string>();
}
