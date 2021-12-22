using FluentValidation;
using SSAS.NET;

namespace Opdex.Platform.WebApi.Validation.Auth;

public class StratisSignatureAuthCallbackQueryValidator : AbstractValidator<StratisSignatureAuthCallbackQuery>
{
    public StratisSignatureAuthCallbackQueryValidator()
    {
        RuleFor(request => request.Uid).NotEmpty().WithMessage("Unique identifier must not be empty.");
        RuleFor(request => request.Exp)
            .GreaterThan(0)
            .WithMessage("Expiration date must be a Unix timestamp.")
            .LessThan(273402300800)
            .WithMessage("Expiration date must be a Unix timestamp.");
    }
}
