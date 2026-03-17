using FamousQuoteQuiz.Application.Common.Models;
using FamousQuoteQuiz.Application.Features.Users.Contracts;

namespace FamousQuoteQuiz.Application.Features.Users;

public interface IUserService
{
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task DisableAsync(int id, CancellationToken cancellationToken);
    Task EnableAsync(int id, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<PagedResult<UserResponse>> ListAsync(ListUsersRequest request, CancellationToken cancellationToken);
}
