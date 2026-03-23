using FamousQuoteQuiz.Application.Features.Quiz;
using FamousQuoteQuiz.Application.Features.Quiz.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FamousQuoteQuiz.Api.Controllers;

[ApiController]
[Route("api/quiz")]
[Authorize(Roles = "User,Admin")]
public sealed class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpPost("sessions")]
    public async Task<ActionResult<GameSessionResponse>> StartSession(
        [FromBody] StartGameSessionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        request.UserId = userId;
        return Ok(await _quizService.StartSessionAsync(userId, request, cancellationToken));
    }

    [HttpGet("sessions/{sessionId:int}/next")]
    public async Task<ActionResult<QuizQuestionResponse>> GetNextQuestion(
        int sessionId,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _quizService.GetNextQuestionAsync(userId, sessionId, cancellationToken));
    }

    [HttpPost("answers/submit")]
    public async Task<ActionResult<SubmitAnswerResponse>> SubmitAnswer(
        [FromBody] SubmitAnswerRequest request,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _quizService.SubmitAnswerAsync(userId, request, cancellationToken));
    }
}
