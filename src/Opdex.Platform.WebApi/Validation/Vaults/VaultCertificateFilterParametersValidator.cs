using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class VaultCertificateFilterParametersValidator : AbstractCursorValidator<VaultCertificateFilterParameters, VaultCertificatesCursor>
{
    public VaultCertificateFilterParametersValidator()
    {
        RuleFor(request => request.Owner).MustBeNetworkAddressOrEmpty().WithMessage("Owner must be valid address.");
        RuleForEach(request => request.Status).MustBeValidEnumValueOrDefault().WithMessage("Status must be valid for the enumeration values.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
