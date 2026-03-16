namespace FamousQuoteQuiz.Application.Features.Achievements.Contracts;

public sealed class ListQuestionAttemptsRequest
{
    public int? UserId { get; set; }
    public bool? IsCorrect { get; set; }
    public string SortBy { get; set; } = "answeredAtUtc";
    public string SortDirection { get; set; } = "desc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
