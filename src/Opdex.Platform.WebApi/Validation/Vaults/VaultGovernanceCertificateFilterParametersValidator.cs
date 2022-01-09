using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class VaultGovernanceCertificateFilterParametersValidator : AbstractCursorValidator<VaultGovernanceCertificateFilterParameters, VaultGovernanceCertificatesCursor>
{
    public VaultGovernanceCertificateFilterParametersValidator()
    {
        RuleFor(request => request.Holder).MustBeNetworkAddressOrEmpty().WithMessage("Holder must be valid address.");
        RuleFor(request => request.Status).MustBeValidEnumValueOrDefault().WithMessage("Status must be valid for the enumeration values.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
