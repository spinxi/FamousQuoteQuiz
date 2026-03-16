using FamousQuoteQuiz.Application.Common.Interfaces;
using FamousQuoteQuiz.Infrastructure.Persistence;
using FamousQuoteQuiz.Infrastructure.Persistence.Seed;
using FamousQuoteQuiz.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamousQuoteQuiz.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IDataSeeder, DataSeeder>();

        return services;
    }
}
