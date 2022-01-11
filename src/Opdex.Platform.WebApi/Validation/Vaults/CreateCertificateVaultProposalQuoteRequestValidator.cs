using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class CreateCertificateVaultProposalQuoteRequestValidator : AbstractValidator<CreateCertificateVaultProposalQuoteRequest>
{
    public CreateCertificateVaultProposalQuoteRequestValidator()
    {
        RuleFor(request => request.Owner).MustBeNetworkAddress().WithMessage("Owner must be valid address.");
        RuleFor(request => request.Amount)
            .MustBeValidSrcValue().WithMessage("Amount must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Amount must be greater than 0.");
        RuleFor(request => request.Description)
            .NotEmpty().WithMessage("Description must not be empty.")
            .MaximumLength(200).WithMessage("Description can contain a maximum of 200 characters.");
    }
}
