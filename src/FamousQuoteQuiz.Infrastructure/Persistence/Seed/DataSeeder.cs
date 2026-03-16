using FamousQuoteQuiz.Application.Common.Interfaces;
using FamousQuoteQuiz.Domain.Entities;
using FamousQuoteQuiz.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FamousQuoteQuiz.Infrastructure.Persistence.Seed;

public sealed class DataSeeder : IDataSeeder
{
    private readonly IAppDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DataSeeder(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var users = new[]
        {
            new User { UserName = "alice", DisplayName = "Alice Johnson", Email = "alice@example.com" },
            new User { UserName = "bob", DisplayName = "Bob Smith", Email = "bob@example.com" },
            new User { UserName = "charlie", DisplayName = "Charlie Brown", Email = "charlie@example.com" }
        };

        _dbContext.Users.AddRange(users);

        var quotes = new[]
        {
            new Quote { Text = "Be yourself; everyone else is already taken.", AuthorName = "Oscar Wilde", IsActive = true },
            new Quote { Text = "I think, therefore I am.", AuthorName = "René Descartes", IsActive = true },
            new Quote { Text = "The secret of getting ahead is getting started.", AuthorName = "Mark Twain", IsActive = true },
            new Quote { Text = "Life is what happens when you're busy making other plans.", AuthorName = "John Lennon", IsActive = true },
            new Quote { Text = "In the middle of difficulty lies opportunity.", AuthorName = "Albert Einstein", IsActive = true },
            new Quote { Text = "To be yourself in a world that is constantly trying to make you something else is the greatest accomplishment.", AuthorName = "Ralph Waldo Emerson", IsActive = true }
        };

        _dbContext.Quotes.AddRange(quotes);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var session = new GameSession
        {
            UserId = users[0].Id,
            Mode = QuizMode.Binary,
            StartedAtUtc = _dateTimeProvider.UtcNow.AddDays(-2),
            EndedAtUtc = _dateTimeProvider.UtcNow.AddDays(-2).AddMinutes(10),
            IsActive = false
        };

        _dbContext.GameSessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var attempts = new[]
        {
            new QuestionAttempt
            {
                GameSessionId = session.Id,
                UserId = users[0].Id,
                QuoteId = quotes[0].Id,
                QuizMode = QuizMode.Binary,
                PresentedAuthorCandidate = "Oscar Wilde",
                UserAnswer = "Yes",
                IsCorrect = true,
                AnsweredAtUtc = _dateTimeProvider.UtcNow.AddDays(-2).AddMinutes(1)
            },
            new QuestionAttempt
            {
                GameSessionId = session.Id,
                UserId = users[0].Id,
                QuoteId = quotes[1].Id,
                QuizMode = QuizMode.Binary,
                PresentedAuthorCandidate = "Mark Twain",
                UserAnswer = "Yes",
                IsCorrect = false,
                AnsweredAtUtc = _dateTimeProvider.UtcNow.AddDays(-2).AddMinutes(2)
            }
        };

        _dbContext.QuestionAttempts.AddRange(attempts);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
