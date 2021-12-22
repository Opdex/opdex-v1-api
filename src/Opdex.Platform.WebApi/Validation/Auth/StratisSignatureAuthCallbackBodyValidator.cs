using FluentValidation;
using SSAS.NET;

namespace Opdex.Platform.WebApi.Validation.Auth;

public class StratisSignatureAuthCallbackBodyValidator : AbstractValidator<StratisSignatureAuthCallbackBody>
{
    public StratisSignatureAuthCallbackBodyValidator()
    {
        RuleFor(request => request.Signature)
            .NotEmpty().WithMessage("Signature must not be empty.")
            .MustBeBase64Encoded().WithMessage("Signature must be valid base-64 encoded string.");
        RuleFor(request => request.PublicKey)
            .MustBeNetworkAddress().WithMessage("Public key must be a valid address.");
    }
}
