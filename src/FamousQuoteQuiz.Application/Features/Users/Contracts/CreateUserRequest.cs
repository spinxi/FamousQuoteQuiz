namespace FamousQuoteQuiz.Application.Features.Users.Contracts;

public sealed class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
}
