using FamousQuoteQuiz.Domain.Common;
using FamousQuoteQuiz.Domain.Enums;

namespace FamousQuoteQuiz.Domain.Entities;

public sealed class GameSession : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public QuizMode Mode { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<QuestionAttempt> QuestionAttempts { get; set; } = new List<QuestionAttempt>();
}
