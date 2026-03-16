using FamousQuoteQuiz.Application.Features.Quiz;
using FamousQuoteQuiz.Application.Features.Quiz.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FamousQuoteQuiz.Api.Controllers;

[ApiController]
[Route("api/quiz")]
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
        => Ok(await _quizService.StartSessionAsync(request, cancellationToken));

    [HttpGet("sessions/{sessionId:int}/next")]
    public async Task<ActionResult<QuizQuestionResponse>> GetNextQuestion(
        int sessionId,
        CancellationToken cancellationToken)
        => Ok(await _quizService.GetNextQuestionAsync(sessionId, cancellationToken));

    [HttpPost("answers/submit")]
    public async Task<ActionResult<SubmitAnswerResponse>> SubmitAnswer(
        [FromBody] SubmitAnswerRequest request,
        CancellationToken cancellationToken)
        => Ok(await _quizService.SubmitAnswerAsync(request, cancellationToken));
}
