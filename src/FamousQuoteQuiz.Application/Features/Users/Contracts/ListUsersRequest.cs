namespace FamousQuoteQuiz.Application.Features.Users.Contracts;

public sealed class ListUsersRequest
{
    public string? Search { get; set; }
    public bool? IsDisabled { get; set; }
    public string SortBy { get; set; } = "displayName";
    public string SortDirection { get; set; } = "asc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
