using FamousQuoteQuiz.Domain.Common;

namespace FamousQuoteQuiz.Domain.Entities;

public sealed class User : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    public ICollection<QuestionAttempt> QuestionAttempts { get; set; } = new List<QuestionAttempt>();
}
