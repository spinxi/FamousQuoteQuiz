using FamousQuoteQuiz.Domain.Enums;

namespace FamousQuoteQuiz.Application.Features.Achievements.Contracts;

public sealed class QuestionAttemptResponse
{
    public int Id { get; init; }
    public int SessionId { get; init; }
    public int UserId { get; init; }
    public string UserDisplayName { get; init; } = string.Empty;
    public int QuoteId { get; init; }
    public string QuoteText { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public QuizMode QuizMode { get; init; }
    public string? PresentedAuthorCandidate { get; init; }
    public string? PresentedOptionsJson { get; init; }
    public string UserAnswer { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
    public DateTime AnsweredAtUtc { get; init; }
}
