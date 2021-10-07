using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults
{
    public class RevokeVaultCertificatesQuoteRequestValidator : AbstractValidator<RevokeVaultCertificatesQuoteRequest>
    {
        public RevokeVaultCertificatesQuoteRequestValidator()
        {
            RuleFor(request => request.Holder).MustBeSmartContractAddress();
        }
    }
}
