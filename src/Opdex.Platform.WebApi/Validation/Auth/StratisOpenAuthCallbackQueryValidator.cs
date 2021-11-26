using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Auth;

namespace Opdex.Platform.WebApi.Validation.Auth
{
    public class StratisOpenAuthCallbackQueryValidator : AbstractValidator<StratisOpenAuthCallbackQuery>
    {
        public StratisOpenAuthCallbackQueryValidator()
        {
            RuleFor(request => request.Uid).NotEmpty();
            RuleFor(request => request.Exp).GreaterThan(0).LessThan(273402300800);
        }
    }
}