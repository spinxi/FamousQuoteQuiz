using FamousQuoteQuiz.Application.Common.Interfaces;

namespace FamousQuoteQuiz.Application.Tests;

internal sealed class TestDateTimeProvider : IDateTimeProvider
{
    public TestDateTimeProvider(DateTime utcNow)
    {
        UtcNow = utcNow;
    }

    public DateTime UtcNow { get; }
}
