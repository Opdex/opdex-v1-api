using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class VaultCertificateFilterParametersValidator : AbstractCursorValidator<VaultCertificateFilterParameters, VaultCertificatesCursor>
{
    public VaultCertificateFilterParametersValidator()
    {
        RuleFor(filter => filter.Holder).MustBeNetworkAddressOrEmpty();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}