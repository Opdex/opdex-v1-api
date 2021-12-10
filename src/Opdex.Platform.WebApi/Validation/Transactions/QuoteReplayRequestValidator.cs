using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Transactions;

namespace Opdex.Platform.WebApi.Validation.Transactions;

public class QuoteReplayRequestValidator : AbstractValidator<QuoteReplayRequest>
{
    public QuoteReplayRequestValidator()
    {
        RuleFor(request => request.Quote).NotEmpty().MustBeBase64Encoded();
    }
}