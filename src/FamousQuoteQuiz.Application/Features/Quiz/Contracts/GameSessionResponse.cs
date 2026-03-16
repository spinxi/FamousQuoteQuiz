using FamousQuoteQuiz.Domain.Enums;

namespace FamousQuoteQuiz.Application.Features.Quiz.Contracts;

public sealed class GameSessionResponse
{
    public int SessionId { get; init; }
    public int UserId { get; init; }
    public QuizMode Mode { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public bool IsActive { get; init; }
}
