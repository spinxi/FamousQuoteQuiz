using FamousQuoteQuiz.Application.Common.Models;
using FamousQuoteQuiz.Application.Features.Achievements;
using FamousQuoteQuiz.Application.Features.Achievements.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FamousQuoteQuiz.Api.Controllers;

[ApiController]
[Route("api/achievements")]
public sealed class AchievementsController : ControllerBase
{
    private readonly IQuestionAttemptService _questionAttemptService;

    public AchievementsController(IQuestionAttemptService questionAttemptService)
    {
        _questionAttemptService = questionAttemptService;
    }

    [HttpGet("attempts")]
    public async Task<ActionResult<PagedResult<QuestionAttemptResponse>>> GetAttempts(
        [FromQuery] ListQuestionAttemptsRequest request,
        CancellationToken cancellationToken)
        => Ok(await _questionAttemptService.ListAsync(request, cancellationToken));
}
