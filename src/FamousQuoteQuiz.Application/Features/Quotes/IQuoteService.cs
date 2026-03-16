using FamousQuoteQuiz.Application.Common.Models;
using FamousQuoteQuiz.Application.Features.Quotes.Contracts;

namespace FamousQuoteQuiz.Application.Features.Quotes;

public interface IQuoteService
{
    Task<QuoteResponse> CreateAsync(CreateQuoteRequest request, CancellationToken cancellationToken);
    Task<QuoteResponse> UpdateAsync(int id, UpdateQuoteRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<PagedResult<QuoteResponse>> ListAsync(ListQuotesRequest request, CancellationToken cancellationToken);
}
