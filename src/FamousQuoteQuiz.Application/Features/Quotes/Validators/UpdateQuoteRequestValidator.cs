using FamousQuoteQuiz.Application.Features.Quotes.Contracts;
using FluentValidation;

namespace FamousQuoteQuiz.Application.Features.Quotes.Validators;

public sealed class UpdateQuoteRequestValidator : AbstractValidator<UpdateQuoteRequest>
{
    public UpdateQuoteRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(200);
    }
}
