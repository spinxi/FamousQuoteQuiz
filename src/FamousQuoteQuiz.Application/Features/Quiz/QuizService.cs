using System.Text.Json;
using FamousQuoteQuiz.Application.Common.Interfaces;
using FamousQuoteQuiz.Application.Features.Quiz.Contracts;
using FamousQuoteQuiz.Domain.Entities;
using FamousQuoteQuiz.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FamousQuoteQuiz.Application.Features.Quiz;

public sealed class QuizService : IQuizService
{
    private readonly IAppDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly Random _random = new();

    public QuizService(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GameSessionResponse> StartSessionAsync(StartGameSessionRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{request.UserId}' was not found.");

        if (user.IsDisabled)
        {
            throw new InvalidOperationException("Disabled users cannot start a game session.");
        }

        var activeSessions = await _dbContext.GameSessions
            .Where(x => x.UserId == request.UserId && x.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var activeSession in activeSessions)
        {
            activeSession.IsActive = false;
            activeSession.EndedAtUtc = _dateTimeProvider.UtcNow;
        }

        var entity = new GameSession
        {
            UserId = request.UserId,
            Mode = request.Mode,
            StartedAtUtc = _dateTimeProvider.UtcNow,
            IsActive = true
        };

        _dbContext.GameSessions.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GameSessionResponse
        {
            SessionId = entity.Id,
            UserId = entity.UserId,
            Mode = entity.Mode,
            StartedAtUtc = entity.StartedAtUtc,
            IsActive = entity.IsActive
        };
    }

    public async Task<QuizQuestionResponse> GetNextQuestionAsync(int sessionId, CancellationToken cancellationToken)
    {
        var session = await _dbContext.GameSessions
            .Include(x => x.User)
            .Include(x => x.QuestionAttempts)
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken)
            ?? throw new KeyNotFoundException($"Session with id '{sessionId}' was not found.");

        if (!session.IsActive)
        {
            throw new InvalidOperationException("Session is not active.");
        }

        if (session.User.IsDisabled || session.User.IsDeleted)
        {
            throw new InvalidOperationException("User is not allowed to continue the quiz.");
        }

        var attemptedQuoteIds = session.QuestionAttempts.Select(x => x.QuoteId).ToHashSet();

        var nextQuote = await _dbContext.Quotes
            .AsNoTracking()
            .Where(x => x.IsActive && !attemptedQuoteIds.Contains(x.Id))
            .OrderBy(x => Guid.NewGuid())
            .FirstOrDefaultAsync(cancellationToken);

        if (nextQuote is null)
        {
            session.IsActive = false;
            session.EndedAtUtc = _dateTimeProvider.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            throw new InvalidOperationException("No more active quotes are available for this session.");
        }

        if (session.Mode == QuizMode.Binary)
        {
            var candidateAuthor = await GenerateBinaryCandidateAsync(nextQuote, cancellationToken);

            return new QuizQuestionResponse
            {
                SessionId = session.Id,
                QuoteId = nextQuote.Id,
                QuoteText = nextQuote.Text,
                Mode = session.Mode,
                CandidateAuthor = candidateAuthor,
                AnswerOptions = new[] { "Yes", "No" }
            };
        }

        var options = await GenerateMultipleChoiceAnswersAsync(nextQuote, cancellationToken);

        return new QuizQuestionResponse
        {
            SessionId = session.Id,
            QuoteId = nextQuote.Id,
            QuoteText = nextQuote.Text,
            Mode = session.Mode,
            AnswerOptions = options
        };
    }

    public async Task<SubmitAnswerResponse> SubmitAnswerAsync(SubmitAnswerRequest request, CancellationToken cancellationToken)
    {
        var session = await _dbContext.GameSessions
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken)
            ?? throw new KeyNotFoundException($"Session with id '{request.SessionId}' was not found.");

        if (!session.IsActive)
        {
            throw new InvalidOperationException("Session is not active.");
        }

        var quote = await _dbContext.Quotes
            .FirstOrDefaultAsync(x => x.Id == request.QuoteId && x.IsActive, cancellationToken)
            ?? throw new KeyNotFoundException($"Quote with id '{request.QuoteId}' was not found.");

        var alreadyAnswered = await _dbContext.QuestionAttempts.AnyAsync(
            x => x.GameSessionId == request.SessionId && x.QuoteId == request.QuoteId,
            cancellationToken);

        if (alreadyAnswered)
        {
            throw new InvalidOperationException("This quote has already been answered in the current session.");
        }

        var normalizedAnswer = request.UserAnswer.Trim();

        var isCorrect = request.Mode switch
        {
            QuizMode.Binary => IsBinaryAnswerCorrect(quote.AuthorName, request.CandidateAuthor, normalizedAnswer),
            QuizMode.MultipleChoice => string.Equals(normalizedAnswer, quote.AuthorName, StringComparison.OrdinalIgnoreCase),
            _ => false
        };

        var attempt = new QuestionAttempt
        {
            GameSessionId = session.Id,
            UserId = session.UserId,
            QuoteId = quote.Id,
            QuizMode = request.Mode,
            PresentedAuthorCandidate = request.CandidateAuthor,
            PresentedOptionsJson = request.PresentedOptions.Any() ? JsonSerializer.Serialize(request.PresentedOptions) : null,
            UserAnswer = normalizedAnswer,
            IsCorrect = isCorrect,
            AnsweredAtUtc = _dateTimeProvider.UtcNow
        };

        _dbContext.QuestionAttempts.Add(attempt);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SubmitAnswerResponse
        {
            IsCorrect = isCorrect,
            CorrectAuthor = quote.AuthorName,
            Message = isCorrect
                ? $"Correct! The right answer is: {quote.AuthorName}."
                : $"Sorry, you are wrong! The right answer is: {quote.AuthorName}."
        };
    }

    private async Task<string> GenerateBinaryCandidateAsync(Quote quote, CancellationToken cancellationToken)
    {
        var useCorrectAuthor = _random.Next(0, 2) == 0;
        if (useCorrectAuthor)
        {
            return quote.AuthorName;
        }

        var otherAuthors = await _dbContext.Quotes
            .AsNoTracking()
            .Where(x => x.IsActive && x.AuthorName != quote.AuthorName)
            .Select(x => x.AuthorName)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (otherAuthors.Count == 0)
        {
            return quote.AuthorName;
        }

        return otherAuthors[_random.Next(otherAuthors.Count)];
    }

    private async Task<IReadOnlyCollection<string>> GenerateMultipleChoiceAnswersAsync(Quote quote, CancellationToken cancellationToken)
    {
        var distractors = await _dbContext.Quotes
            .AsNoTracking()
            .Where(x => x.IsActive && x.AuthorName != quote.AuthorName)
            .Select(x => x.AuthorName)
            .Distinct()
            .OrderBy(x => Guid.NewGuid())
            .Take(2)
            .ToListAsync(cancellationToken);

        if (distractors.Count < 2)
        {
            throw new InvalidOperationException("At least 3 unique authors are required for multiple choice mode.");
        }

        return distractors.Append(quote.AuthorName)
            .OrderBy(_ => Guid.NewGuid())
            .ToArray();
    }

    private static bool IsBinaryAnswerCorrect(string correctAuthor, string? candidateAuthor, string userAnswer)
    {
        var candidateIsCorrect = string.Equals(correctAuthor, candidateAuthor, StringComparison.OrdinalIgnoreCase);

        return (candidateIsCorrect && userAnswer.Equals("Yes", StringComparison.OrdinalIgnoreCase))
               || (!candidateIsCorrect && userAnswer.Equals("No", StringComparison.OrdinalIgnoreCase));
    }
}
