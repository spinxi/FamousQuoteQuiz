namespace FamousQuoteQuiz.Infrastructure.Persistence.Seed;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
