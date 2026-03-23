using FamousQuoteQuiz.Application.Common.Models;
using FamousQuoteQuiz.Application.Features.Users;
using FamousQuoteQuiz.Application.Features.Users.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamousQuoteQuiz.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetUsers(
        [FromQuery] ListUsersRequest request,
        CancellationToken cancellationToken)
        => Ok(await _userService.ListAsync(request, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
        => Ok(await _userService.CreateAsync(request, cancellationToken));

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(
        int id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
        => Ok(await _userService.UpdateAsync(id, request, cancellationToken));

    [HttpPatch("{id:int}/disable")]
    public async Task<IActionResult> DisableUser(int id, CancellationToken cancellationToken)
    {
        await _userService.DisableAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}/enable")]
    public async Task<IActionResult> EnableUser(int id, CancellationToken cancellationToken)
    {
        await _userService.EnableAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
