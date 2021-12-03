using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Auth;

namespace Opdex.Platform.WebApi.Validation.Auth;

public class StratisOpenAuthCallbackBodyValidator : AbstractValidator<StratisOpenAuthCallbackBody>
{
    public StratisOpenAuthCallbackBodyValidator()
    {
        RuleFor(request => request.Signature).NotEmpty().MustBeBase64Encoded();
        RuleFor(request => request.PublicKey).MustBeNetworkAddress();
    }
}