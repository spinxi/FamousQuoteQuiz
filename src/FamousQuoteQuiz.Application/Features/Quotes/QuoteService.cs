using FamousQuoteQuiz.Application.Common.Interfaces;
using FamousQuoteQuiz.Application.Common.Models;
using FamousQuoteQuiz.Application.Features.Quotes.Contracts;
using FamousQuoteQuiz.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamousQuoteQuiz.Application.Features.Quotes;

public sealed class QuoteService : IQuoteService
{
    private readonly IAppDbContext _dbContext;

    public QuoteService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuoteResponse> CreateAsync(CreateQuoteRequest request, CancellationToken cancellationToken)
    {
        var entity = new Quote
        {
            Text = request.Text.Trim(),
            AuthorName = request.AuthorName.Trim(),
            IsActive = request.IsActive
        };

        _dbContext.Quotes.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<QuoteResponse> UpdateAsync(int id, UpdateQuoteRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Quotes
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Quote with id '{id}' was not found.");

        entity.Text = request.Text.Trim();
        entity.AuthorName = request.AuthorName.Trim();
        entity.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Quotes
            .Include(x => x.QuestionAttempts)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Quote with id '{id}' was not found.");

        if (entity.QuestionAttempts.Any())
        {
            throw new InvalidOperationException("Quote cannot be deleted because it is referenced by question attempts.");
        }

        _dbContext.Quotes.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<QuoteResponse>> ListAsync(ListQuotesRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Quotes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Text.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.AuthorName))
        {
            var author = request.AuthorName.Trim();
            query = query.Where(x => x.AuthorName.Contains(author));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        var desc = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        query = request.SortBy.Trim().ToLowerInvariant() switch
        {
            "text" => desc ? query.OrderByDescending(x => x.Text).ThenByDescending(x => x.Id) : query.OrderBy(x => x.Text).ThenBy(x => x.Id),
            "createdatutc" => desc ? query.OrderByDescending(x => x.CreatedAtUtc).ThenByDescending(x => x.Id) : query.OrderBy(x => x.CreatedAtUtc).ThenBy(x => x.Id),
            "status" => desc ? query.OrderByDescending(x => x.IsActive).ThenByDescending(x => x.Id) : query.OrderBy(x => x.IsActive).ThenBy(x => x.Id),
            _ => desc ? query.OrderByDescending(x => x.AuthorName).ThenByDescending(x => x.Id) : query.OrderBy(x => x.AuthorName).ThenBy(x => x.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new QuoteResponse
            {
                Id = x.Id,
                Text = x.Text,
                AuthorName = x.AuthorName,
                IsActive = x.IsActive,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<QuoteResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private static QuoteResponse Map(Quote entity) => new()
    {
        Id = entity.Id,
        Text = entity.Text,
        AuthorName = entity.AuthorName,
        IsActive = entity.IsActive,
        CreatedAtUtc = entity.CreatedAtUtc
    };
}
