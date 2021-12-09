using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class CreateCertificateVaultProposalQuoteRequestValidator : AbstractValidator<CreateCertificateVaultProposalQuoteRequest>
{
    public CreateCertificateVaultProposalQuoteRequestValidator()
    {
        RuleFor(request => request.Owner).MustBeNetworkAddress();
        RuleFor(request => request.Amount).MustBeValidSrcValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(200);
    }
}
