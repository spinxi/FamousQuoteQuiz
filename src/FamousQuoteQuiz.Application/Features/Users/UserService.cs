using FamousQuoteQuiz.Application.Common.Interfaces;
using FamousQuoteQuiz.Application.Common.Models;
using FamousQuoteQuiz.Application.Features.Users.Contracts;
using FamousQuoteQuiz.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamousQuoteQuiz.Application.Features.Users;

public sealed class UserService : IUserService
{
    private readonly IAppDbContext _dbContext;
    private readonly IPasswordService _passwordService;

    public UserService(IAppDbContext dbContext, IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var normalizedUserName = request.UserName.Trim();

        var exists = await _dbContext.Users
            .AnyAsync(x => x.UserName == normalizedUserName && !x.IsDeleted, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"User with username '{normalizedUserName}' already exists.");
        }

        var entity = new User
        {
            UserName = normalizedUserName,
            PasswordHash = _passwordService.HashPassword(request.Password),
            DisplayName = request.DisplayName.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Role = request.Role
        };

        _dbContext.Users.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        entity.DisplayName = request.DisplayName.Trim();
        entity.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        entity.Role = request.Role;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            entity.PasswordHash = _passwordService.HashPassword(request.Password);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task DisableAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        entity.IsDisabled = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task EnableAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        entity.IsDisabled = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Users
            .Include(x => x.GameSessions)
            .Include(x => x.QuestionAttempts)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{id}' was not found.");

        if (entity.GameSessions.Any() || entity.QuestionAttempts.Any())
        {
            throw new InvalidOperationException("User cannot be deleted because related game history exists.");
        }

        entity.IsDeleted = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<UserResponse>> ListAsync(ListUsersRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                x.UserName.Contains(term) ||
                x.DisplayName.Contains(term) ||
                (x.Email != null && x.Email.Contains(term)));
        }

        if (request.IsDisabled.HasValue)
        {
            query = query.Where(x => x.IsDisabled == request.IsDisabled.Value);
        }

        var desc = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        query = request.SortBy.Trim().ToLowerInvariant() switch
        {
            "username" => desc ? query.OrderByDescending(x => x.UserName).ThenByDescending(x => x.Id) : query.OrderBy(x => x.UserName).ThenBy(x => x.Id),
            "createdatutc" => desc ? query.OrderByDescending(x => x.CreatedAtUtc).ThenByDescending(x => x.Id) : query.OrderBy(x => x.CreatedAtUtc).ThenBy(x => x.Id),
            "status" => desc ? query.OrderByDescending(x => x.IsDisabled).ThenByDescending(x => x.Id) : query.OrderBy(x => x.IsDisabled).ThenBy(x => x.Id),
            _ => desc ? query.OrderByDescending(x => x.DisplayName).ThenByDescending(x => x.Id) : query.OrderBy(x => x.DisplayName).ThenBy(x => x.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                UserName = x.UserName,
                DisplayName = x.DisplayName,
                Email = x.Email,
                Role = x.Role,
                IsDisabled = x.IsDisabled,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<UserResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    private static UserResponse Map(User entity) => new()
    {
        Id = entity.Id,
        UserName = entity.UserName,
        DisplayName = entity.DisplayName,
        Email = entity.Email,
        Role = entity.Role,
        IsDisabled = entity.IsDisabled,
        CreatedAtUtc = entity.CreatedAtUtc
    };
}
