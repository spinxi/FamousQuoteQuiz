using FamousQuoteQuiz.Domain.Common;
using FamousQuoteQuiz.Domain.Enums;

namespace FamousQuoteQuiz.Domain.Entities;

public sealed class QuestionAttempt : BaseEntity
{
    public int GameSessionId { get; set; }
    public GameSession GameSession { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int QuoteId { get; set; }
    public Quote Quote { get; set; } = null!;

    public QuizMode QuizMode { get; set; }
    public string? PresentedAuthorCandidate { get; set; }
    public string? PresentedOptionsJson { get; set; }
    public string UserAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime AnsweredAtUtc { get; set; }
}
