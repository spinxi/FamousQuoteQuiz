namespace FamousQuoteQuiz.Application.Features.Users.Contracts;

public sealed class UpdateUserRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
}
