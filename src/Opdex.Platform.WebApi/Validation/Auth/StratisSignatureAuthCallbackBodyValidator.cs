using FluentValidation;
using SSAS.NET;

namespace Opdex.Platform.WebApi.Validation.Auth;

public class StratisSignatureAuthCallbackBodyValidator : AbstractValidator<StratisSignatureAuthCallbackBody>
{
    public StratisSignatureAuthCallbackBodyValidator()
    {
        RuleFor(request => request.Signature).NotEmpty().MustBeBase64Encoded();
        RuleFor(request => request.PublicKey).MustBeNetworkAddress();
    }
}