using FamousQuoteQuiz.Application.Features.Quiz.Contracts;

namespace FamousQuoteQuiz.Application.Features.Quiz;

public interface IQuizService
{
    Task<GameSessionResponse> StartSessionAsync(int currentUserId, StartGameSessionRequest request, CancellationToken cancellationToken);
    Task<QuizQuestionResponse> GetNextQuestionAsync(int currentUserId, int sessionId, CancellationToken cancellationToken);
    Task<SubmitAnswerResponse> SubmitAnswerAsync(int currentUserId, SubmitAnswerRequest request, CancellationToken cancellationToken);
}
