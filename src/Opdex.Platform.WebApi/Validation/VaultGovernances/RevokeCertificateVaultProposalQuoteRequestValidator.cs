using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class RevokeCertificateVaultProposalQuoteRequestValidator : AbstractValidator<RevokeCertificateVaultProposalQuoteRequest>
{
    public RevokeCertificateVaultProposalQuoteRequestValidator()
    {
        RuleFor(request => request.Owner).MustBeNetworkAddress();
        RuleFor(request => request.Description).NotEmpty().MaximumLength(200);
    }
}