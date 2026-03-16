namespace FamousQuoteQuiz.Application.Features.Quiz.Contracts;

public sealed class SubmitAnswerResponse
{
    public bool IsCorrect { get; init; }
    public string CorrectAuthor { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
