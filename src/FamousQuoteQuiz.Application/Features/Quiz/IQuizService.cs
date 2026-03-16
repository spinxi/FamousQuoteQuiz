using FamousQuoteQuiz.Application.Features.Quiz.Contracts;

namespace FamousQuoteQuiz.Application.Features.Quiz;

public interface IQuizService
{
    Task<GameSessionResponse> StartSessionAsync(StartGameSessionRequest request, CancellationToken cancellationToken);
    Task<QuizQuestionResponse> GetNextQuestionAsync(int sessionId, CancellationToken cancellationToken);
    Task<SubmitAnswerResponse> SubmitAnswerAsync(SubmitAnswerRequest request, CancellationToken cancellationToken);
}
