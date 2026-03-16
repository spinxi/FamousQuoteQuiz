using FamousQuoteQuiz.Domain.Common;

namespace FamousQuoteQuiz.Domain.Entities;

public sealed class Quote : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<QuestionAttempt> QuestionAttempts { get; set; } = new List<QuestionAttempt>();
}
