using FamousQuoteQuiz.Domain.Enums;

namespace FamousQuoteQuiz.Application.Features.Users.Contracts;

public sealed class UserResponse
{
    public int Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public UserRole Role { get; init; }
    public bool IsDisabled { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
