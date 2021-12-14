using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Proposals;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultProposalFilterParametersValidator : AbstractCursorValidator<VaultProposalFilterParameters, VaultProposalsCursor>
{
    public VaultProposalFilterParametersValidator()
    {
        RuleFor(filter => filter.Status).MustBeValidEnumValueOrDefault();
        RuleFor(filter => filter.Type).MustBeValidEnumValueOrDefault();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}
