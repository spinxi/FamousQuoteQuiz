using FamousQuoteQuiz.Application.Features.Quiz;
using FamousQuoteQuiz.Application.Features.Quiz.Contracts;
using FamousQuoteQuiz.Domain.Entities;
using FamousQuoteQuiz.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FamousQuoteQuiz.Application.Tests;

public sealed class QuizServiceTests
{
    private static readonly DateTime FixedUtcNow = new(2026, 3, 17, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task StartSessionAsync_DisablesPreviousActiveSessionsForUser()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var user = new User
        {
            UserName = "alice",
            DisplayName = "Alice Johnson"
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var oldSession = new GameSession
        {
            UserId = user.Id,
            Mode = QuizMode.Binary,
            StartedAtUtc = FixedUtcNow.AddMinutes(-15),
            IsActive = true
        };
        dbContext.GameSessions.Add(oldSession);
        await dbContext.SaveChangesAsync();

        var service = new QuizService(dbContext, new TestDateTimeProvider(FixedUtcNow));

        var response = await service.StartSessionAsync(new StartGameSessionRequest
        {
            UserId = user.Id,
            Mode = QuizMode.MultipleChoice
        }, CancellationToken.None);

        var sessions = await dbContext.GameSessions
            .OrderBy(x => x.Id)
            .ToListAsync();

        Assert.Equal(2, sessions.Count);
        Assert.False(sessions[0].IsActive);
        Assert.Equal(FixedUtcNow, sessions[0].EndedAtUtc);
        Assert.True(sessions[1].IsActive);
        Assert.Equal(sessions[1].Id, response.SessionId);
        Assert.Equal(QuizMode.MultipleChoice, response.Mode);
    }

    [Fact]
    public async Task StartSessionAsync_ThrowsWhenUserIsDisabled()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var user = new User
        {
            UserName = "bob",
            DisplayName = "Bob Smith",
            IsDisabled = true
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var service = new QuizService(dbContext, new TestDateTimeProvider(FixedUtcNow));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.StartSessionAsync(
            new StartGameSessionRequest
            {
                UserId = user.Id,
                Mode = QuizMode.Binary
            },
            CancellationToken.None));

        Assert.Equal("Disabled users cannot start a game session.", exception.Message);
    }

    [Theory]
    [InlineData("Oscar Wilde", true, "Correct! The right answer is: Oscar Wilde.")]
    [InlineData("Mark Twain", false, "Sorry, you are wrong! The right answer is: Oscar Wilde.")]
    public async Task SubmitAnswerAsync_ReturnsExpectedCorrectnessAndMessage(
        string userAnswer,
        bool expectedCorrectness,
        string expectedMessage)
    {
        await using var dbContext = TestDbContextFactory.Create();
        var user = new User
        {
            UserName = "charlie",
            DisplayName = "Charlie Brown"
        };
        var quote = new Quote
        {
            Text = "Be yourself; everyone else is already taken.",
            AuthorName = "Oscar Wilde",
            IsActive = true
        };
        dbContext.Users.Add(user);
        dbContext.Quotes.Add(quote);
        await dbContext.SaveChangesAsync();

        var session = new GameSession
        {
            UserId = user.Id,
            Mode = QuizMode.MultipleChoice,
            StartedAtUtc = FixedUtcNow,
            IsActive = true
        };
        dbContext.GameSessions.Add(session);
        await dbContext.SaveChangesAsync();

        var service = new QuizService(dbContext, new TestDateTimeProvider(FixedUtcNow));

        var response = await service.SubmitAnswerAsync(new SubmitAnswerRequest
        {
            SessionId = session.Id,
            QuoteId = quote.Id,
            Mode = QuizMode.MultipleChoice,
            UserAnswer = userAnswer,
            PresentedOptions = new[] { "Oscar Wilde", "Mark Twain", "Albert Einstein" }
        }, CancellationToken.None);

        var attempt = await dbContext.QuestionAttempts.SingleAsync();

        Assert.Equal(expectedCorrectness, response.IsCorrect);
        Assert.Equal(expectedMessage, response.Message);
        Assert.Equal(expectedCorrectness, attempt.IsCorrect);
        Assert.Equal(userAnswer, attempt.UserAnswer);
    }

    [Fact]
    public async Task GetNextQuestionAsync_EndsSessionWhenNoMoreActiveQuotesAreAvailable()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var user = new User
        {
            UserName = "diana",
            DisplayName = "Diana Prince"
        };
        var quote = new Quote
        {
            Text = "I think, therefore I am.",
            AuthorName = "Rene Descartes",
            IsActive = true
        };
        dbContext.Users.Add(user);
        dbContext.Quotes.Add(quote);
        await dbContext.SaveChangesAsync();

        var session = new GameSession
        {
            UserId = user.Id,
            User = user,
            Mode = QuizMode.Binary,
            StartedAtUtc = FixedUtcNow.AddMinutes(-5),
            IsActive = true
        };
        dbContext.GameSessions.Add(session);
        await dbContext.SaveChangesAsync();

        dbContext.QuestionAttempts.Add(new QuestionAttempt
        {
            GameSessionId = session.Id,
            UserId = user.Id,
            QuoteId = quote.Id,
            QuizMode = QuizMode.Binary,
            UserAnswer = "Yes",
            IsCorrect = true,
            AnsweredAtUtc = FixedUtcNow.AddMinutes(-1)
        });
        await dbContext.SaveChangesAsync();

        var service = new QuizService(dbContext, new TestDateTimeProvider(FixedUtcNow));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetNextQuestionAsync(
            session.Id,
            CancellationToken.None));

        var updatedSession = await dbContext.GameSessions.SingleAsync();

        Assert.Equal("No more active quotes are available for this session.", exception.Message);
        Assert.False(updatedSession.IsActive);
        Assert.Equal(FixedUtcNow, updatedSession.EndedAtUtc);
    }
}
