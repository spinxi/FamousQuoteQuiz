using FamousQuoteQuiz.Application.Common.Models;
using FamousQuoteQuiz.Application.Features.Quotes;
using FamousQuoteQuiz.Application.Features.Quotes.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamousQuoteQuiz.Api.Controllers;

[ApiController]
[Route("api/quotes")]
[Authorize(Roles = "Admin")]
public sealed class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<QuoteResponse>>> GetQuotes(
        [FromQuery] ListQuotesRequest request,
        CancellationToken cancellationToken)
        => Ok(await _quoteService.ListAsync(request, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<QuoteResponse>> CreateQuote(
        [FromBody] CreateQuoteRequest request,
        CancellationToken cancellationToken)
        => Ok(await _quoteService.CreateAsync(request, cancellationToken));

    [HttpPut("{id:int}")]
    public async Task<ActionResult<QuoteResponse>> UpdateQuote(
        int id,
        [FromBody] UpdateQuoteRequest request,
        CancellationToken cancellationToken)
        => Ok(await _quoteService.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteQuote(int id, CancellationToken cancellationToken)
    {
        await _quoteService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
