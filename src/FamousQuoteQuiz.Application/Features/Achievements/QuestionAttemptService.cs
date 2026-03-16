using FamousQuoteQuiz.Application.Common.Interfaces;
using FamousQuoteQuiz.Application.Common.Models;
using FamousQuoteQuiz.Application.Features.Achievements.Contracts;
using FamousQuoteQuiz.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamousQuoteQuiz.Application.Features.Achievements;

public sealed class QuestionAttemptService : IQuestionAttemptService
{
    private readonly IAppDbContext _dbContext;

    public QuestionAttemptService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<QuestionAttemptResponse>> ListAsync(ListQuestionAttemptsRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.QuestionAttempts
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Quote)
            .AsQueryable();

        if (request.UserId.HasValue)
        {
            query = query.Where(x => x.UserId == request.UserId.Value);
        }

        if (request.IsCorrect.HasValue)
        {
            query = query.Where(x => x.IsCorrect == request.IsCorrect.Value);
        }

        query = ApplySorting(query, request.SortBy, request.SortDirection);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new QuestionAttemptResponse
            {
                Id = x.Id,
                SessionId = x.GameSessionId,
                UserId = x.UserId,
                UserDisplayName = x.User.DisplayName,
                QuoteId = x.QuoteId,
                QuoteText = x.Quote.Text,
                AuthorName = x.Quote.AuthorName,
                QuizMode = x.QuizMode,
                PresentedAuthorCandidate = x.PresentedAuthorCandidate,
                PresentedOptionsJson = x.PresentedOptionsJson,
                UserAnswer = x.UserAnswer,
                IsCorrect = x.IsCorrect,
                AnsweredAtUtc = x.AnsweredAtUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<QuestionAttemptResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private static IQueryable<QuestionAttempt> ApplySorting(
        IQueryable<QuestionAttempt> query,
        string sortBy,
        string sortDirection)
    {
        var desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "user" => desc ? query.OrderByDescending(x => x.User.DisplayName) : query.OrderBy(x => x.User.DisplayName),
            "correctness" => desc ? query.OrderByDescending(x => x.IsCorrect) : query.OrderBy(x => x.IsCorrect),
            _ => desc ? query.OrderByDescending(x => x.AnsweredAtUtc) : query.OrderBy(x => x.AnsweredAtUtc)
        };
    }
}
