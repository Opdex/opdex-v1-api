using FluentValidation;
using SSAS.NET;

namespace Opdex.Platform.WebApi.Validation.Auth;

public class StratisSignatureAuthCallbackQueryValidator : AbstractValidator<StratisSignatureAuthCallbackQuery>
{
    public StratisSignatureAuthCallbackQueryValidator()
    {
        RuleFor(request => request.Uid).NotEmpty();
        RuleFor(request => request.Exp).GreaterThan(0).LessThan(273402300800);
    }
}