using FamousQuoteQuiz.Application.Features.Users;
using FamousQuoteQuiz.Infrastructure.Services;
using FamousQuoteQuiz.Domain.Entities;
using FamousQuoteQuiz.Domain.Enums;
using Xunit;

namespace FamousQuoteQuiz.Application.Tests;

public sealed class UserServiceTests
{
    [Fact]
    public async Task DeleteAsync_ThrowsWhenUserHasGameHistory()
    {
        await using var dbContext = TestDbContextFactory.Create();
        var user = new User
        {
            UserName = "eve",
            DisplayName = "Eve Adams"
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        dbContext.GameSessions.Add(new GameSession
        {
            UserId = user.Id,
            Mode = QuizMode.Binary,
            StartedAtUtc = new DateTime(2026, 3, 17, 8, 0, 0, DateTimeKind.Utc),
            IsActive = false
        });
        await dbContext.SaveChangesAsync();

        var service = new UserService(dbContext, new PasswordService());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(
            user.Id,
            CancellationToken.None));

        Assert.Equal("User cannot be deleted because related game history exists.", exception.Message);
    }
}
