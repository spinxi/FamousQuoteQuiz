using FamousQuoteQuiz.Application.Common.Interfaces;

namespace FamousQuoteQuiz.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
