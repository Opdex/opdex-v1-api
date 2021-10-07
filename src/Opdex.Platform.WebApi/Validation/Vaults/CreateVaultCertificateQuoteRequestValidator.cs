using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults
{
    public class CreateVaultCertificateQuoteRequestValidator : AbstractValidator<CreateVaultCertificateQuoteRequest>
    {
        public CreateVaultCertificateQuoteRequestValidator()
        {
            RuleFor(request => request.Amount).GreaterThan(FixedDecimal.Zero);
            RuleFor(request => request.Holder).MustBeSmartContractAddress();
        }
    }
}
