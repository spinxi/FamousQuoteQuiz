using FamousQuoteQuiz.Application.Common.Models;
using FamousQuoteQuiz.Application.Features.Achievements.Contracts;

namespace FamousQuoteQuiz.Application.Features.Achievements;

public interface IQuestionAttemptService
{
    Task<PagedResult<QuestionAttemptResponse>> ListAsync(ListQuestionAttemptsRequest request, CancellationToken cancellationToken);
}
