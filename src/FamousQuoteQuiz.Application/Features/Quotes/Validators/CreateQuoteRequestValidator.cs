using FamousQuoteQuiz.Application.Features.Quotes.Contracts;
using FluentValidation;

namespace FamousQuoteQuiz.Application.Features.Quotes.Validators;

public sealed class CreateQuoteRequestValidator : AbstractValidator<CreateQuoteRequest>
{
    public CreateQuoteRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(200);
    }
}
