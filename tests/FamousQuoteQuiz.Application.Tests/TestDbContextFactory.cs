using FamousQuoteQuiz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamousQuoteQuiz.Application.Tests;

internal static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
