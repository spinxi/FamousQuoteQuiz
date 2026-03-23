using FamousQuoteQuiz.Application.Common.Interfaces;
using FamousQuoteQuiz.Domain.Entities;
using FamousQuoteQuiz.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FamousQuoteQuiz.Infrastructure.Persistence.Seed;

public sealed class DataSeeder : IDataSeeder
{
    private readonly IAppDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordService _passwordService;

    public DataSeeder(IAppDbContext dbContext, IDateTimeProvider dateTimeProvider, IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _passwordService = passwordService;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingUsers = await _dbContext.Users.ToListAsync(cancellationToken);

        if (existingUsers.Count == 0)
        {
            var users = new[]
            {
                new User { UserName = "admin", PasswordHash = _passwordService.HashPassword("Admin123!"), DisplayName = "System Admin", Email = "admin@example.com", Role = UserRole.Admin },
                new User { UserName = "alice", PasswordHash = _passwordService.HashPassword("Alice123!"), DisplayName = "Alice Johnson", Email = "alice@example.com", Role = UserRole.User },
                new User { UserName = "bob", PasswordHash = _passwordService.HashPassword("Bob123!"), DisplayName = "Bob Smith", Email = "bob@example.com", Role = UserRole.User },
                new User { UserName = "charlie", PasswordHash = _passwordService.HashPassword("Charlie123!"), DisplayName = "Charlie Brown", Email = "charlie@example.com", Role = UserRole.User }
            };

            _dbContext.Users.AddRange(users);
            existingUsers = users.ToList();
        }
        else
        {
            foreach (var user in existingUsers.Where(x => string.IsNullOrWhiteSpace(x.PasswordHash)))
            {
                var defaultPassword = user.UserName.Length > 0
                    ? $"{char.ToUpperInvariant(user.UserName[0])}{user.UserName[1..]}123!"
                    : "User123!";

                user.PasswordHash = _passwordService.HashPassword(defaultPassword);
                user.Role = string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase) ? UserRole.Admin : UserRole.User;
            }

            if (existingUsers.All(x => x.Role != UserRole.Admin))
            {
                var admin = new User
                {
                    UserName = "admin",
                    PasswordHash = _passwordService.HashPassword("Admin123!"),
                    DisplayName = "System Admin",
                    Email = "admin@example.com",
                    Role = UserRole.Admin
                };

                _dbContext.Users.Add(admin);
                existingUsers.Add(admin);
            }
        }

        if (!await _dbContext.Quotes.AnyAsync(cancellationToken))
        {
            var quotes = new[]
            {
                new Quote { Text = "Be yourself; everyone else is already taken.", AuthorName = "Oscar Wilde", IsActive = true },
                new Quote { Text = "I think, therefore I am.", AuthorName = "Rene Descartes", IsActive = true },
                new Quote { Text = "The secret of getting ahead is getting started.", AuthorName = "Mark Twain", IsActive = true },
                new Quote { Text = "Life is what happens when you're busy making other plans.", AuthorName = "John Lennon", IsActive = true },
                new Quote { Text = "In the middle of difficulty lies opportunity.", AuthorName = "Albert Einstein", IsActive = true },
                new Quote { Text = "To be yourself in a world that is constantly trying to make you something else is the greatest accomplishment.", AuthorName = "Ralph Waldo Emerson", IsActive = true }
            };

            _dbContext.Quotes.AddRange(quotes);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (await _dbContext.QuestionAttempts.AnyAsync(cancellationToken))
        {
            return;
        }

        var sampleUser = existingUsers.FirstOrDefault(x => x.UserName == "alice") ?? existingUsers.First(x => x.Role == UserRole.User);
        var sampleQuotes = await _dbContext.Quotes.OrderBy(x => x.Id).Take(2).ToListAsync(cancellationToken);

        var session = new GameSession
        {
            UserId = sampleUser.Id,
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
                UserId = sampleUser.Id,
                QuoteId = sampleQuotes[0].Id,
                QuizMode = QuizMode.Binary,
                PresentedAuthorCandidate = "Oscar Wilde",
                UserAnswer = "Yes",
                IsCorrect = true,
                AnsweredAtUtc = _dateTimeProvider.UtcNow.AddDays(-2).AddMinutes(1)
            },
            new QuestionAttempt
            {
                GameSessionId = session.Id,
                UserId = sampleUser.Id,
                QuoteId = sampleQuotes[1].Id,
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
