using FamousQuoteQuiz.Application.Features.Achievements;
using FamousQuoteQuiz.Application.Features.Auth;
using FamousQuoteQuiz.Application.Features.Quotes;
using FamousQuoteQuiz.Application.Features.Quiz;
using FamousQuoteQuiz.Application.Features.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FamousQuoteQuiz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IQuoteService, QuoteService>();
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<IQuestionAttemptService, QuestionAttemptService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
