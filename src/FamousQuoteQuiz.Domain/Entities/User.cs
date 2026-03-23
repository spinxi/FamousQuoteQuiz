using FamousQuoteQuiz.Domain.Common;
using FamousQuoteQuiz.Domain.Enums;

namespace FamousQuoteQuiz.Domain.Entities;

public sealed class User : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsDisabled { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    public ICollection<QuestionAttempt> QuestionAttempts { get; set; } = new List<QuestionAttempt>();
}
