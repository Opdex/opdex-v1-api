using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultGovernanceCertificateFilterParametersValidator : AbstractCursorValidator<VaultGovernanceCertificateFilterParameters, VaultGovernanceCertificatesCursor>
{
    public VaultGovernanceCertificateFilterParametersValidator()
    {
        RuleFor(request => request.Holder).MustBeNetworkAddressOrEmpty();
        RuleFor(request => request.Status).MustBeValidEnumValueOrDefault();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}
