using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Tokens;

namespace Opdex.Platform.WebApi.Validation.Tokens;

public class AddTokenRequestValidator : AbstractValidator<AddTokenRequest>
{
    public AddTokenRequestValidator()
    {
        RuleFor(request => request.Token)
            .MustBeNetworkAddress().WithMessage("Token must be valid address.");
    }
}
