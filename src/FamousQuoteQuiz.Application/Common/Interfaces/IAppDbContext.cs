using FamousQuoteQuiz.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamousQuoteQuiz.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Quote> Quotes { get; }
    DbSet<GameSession> GameSessions { get; }
    DbSet<QuestionAttempt> QuestionAttempts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
