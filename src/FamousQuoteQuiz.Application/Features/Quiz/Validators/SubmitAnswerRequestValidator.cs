using FamousQuoteQuiz.Application.Features.Quiz.Contracts;
using FamousQuoteQuiz.Domain.Enums;
using FluentValidation;

namespace FamousQuoteQuiz.Application.Features.Quiz.Validators;

public sealed class SubmitAnswerRequestValidator : AbstractValidator<SubmitAnswerRequest>
{
    public SubmitAnswerRequestValidator()
    {
        RuleFor(x => x.SessionId).GreaterThan(0);
        RuleFor(x => x.QuoteId).GreaterThan(0);
        RuleFor(x => x.UserAnswer).NotEmpty();

        When(x => x.Mode == QuizMode.Binary, () =>
        {
            RuleFor(x => x.CandidateAuthor).NotEmpty();
            RuleFor(x => x.UserAnswer)
                .Must(x => x.Equals("Yes", StringComparison.OrdinalIgnoreCase)
                           || x.Equals("No", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Binary mode answer must be Yes or No.");
        });

        When(x => x.Mode == QuizMode.MultipleChoice, () =>
        {
            RuleFor(x => x.PresentedOptions)
                .Must(x => x.Count == 3)
                .WithMessage("Multiple choice mode requires exactly 3 presented options.");
        });
    }
}
