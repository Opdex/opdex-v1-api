using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultGovernanceCertificateFilterParametersValidator : AbstractCursorValidator<VaultGovernanceCertificateFilterParameters, VaultGovernanceCertificatesCursor>
{
    public VaultGovernanceCertificateFilterParametersValidator()
    {
        RuleFor(request => request.Holder).MustBeNetworkAddressOrEmpty().WithMessage("Holder must be valid address.");
        RuleFor(request => request.Status).MustBeValidEnumValueOrDefault().WithMessage("Status must be valid for the enumeration values.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
