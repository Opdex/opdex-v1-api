using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class RevokeCertificateVaultProposalQuoteRequestValidator : AbstractValidator<RevokeCertificateVaultProposalQuoteRequest>
{
    public RevokeCertificateVaultProposalQuoteRequestValidator()
    {
        RuleFor(request => request.Owner).MustBeNetworkAddress().WithMessage("Owner must be valid address.");
        RuleFor(request => request.Description)
            .NotEmpty().WithMessage("Description must not be empty.")
            .MaximumLength(200).WithMessage("Description can contain a maximum of 200 characters.");
    }
}
