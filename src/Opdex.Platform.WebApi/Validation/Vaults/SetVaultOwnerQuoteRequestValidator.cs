using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults
{
    public class SetVaultOwnerQuoteRequestValidator : AbstractValidator<SetVaultOwnerQuoteRequest>
    {
        public SetVaultOwnerQuoteRequestValidator()
        {
            RuleFor(request => request.Owner).MustBeNetworkAddress();
        }
    }
}
