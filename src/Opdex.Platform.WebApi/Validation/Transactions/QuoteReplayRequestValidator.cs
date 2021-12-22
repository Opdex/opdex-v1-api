using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Transactions;

namespace Opdex.Platform.WebApi.Validation.Transactions;

public class QuoteReplayRequestValidator : AbstractValidator<QuoteReplayRequest>
{
    public QuoteReplayRequestValidator()
    {
        RuleFor(request => request.Quote)
            .NotEmpty().WithMessage("Quote must not be empty.")
            .MustBeBase64Encoded().WithMessage("Quote must be valid base-64 encoded string.");
    }
}
