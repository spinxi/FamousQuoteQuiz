using FamousQuoteQuiz.Domain.Enums;

namespace FamousQuoteQuiz.Application.Features.Quiz.Contracts;

public sealed class StartGameSessionRequest
{
    public int UserId { get; set; }
    public QuizMode Mode { get; set; } = QuizMode.Binary;
}
