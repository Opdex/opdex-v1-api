using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class VaultProposalFilterParametersValidator : AbstractCursorValidator<VaultProposalFilterParameters, VaultProposalsCursor>
{
    public VaultProposalFilterParametersValidator()
    {
        RuleFor(filter => filter.Status).MustBeValidEnumValueOrDefault().WithMessage("Status must be valid or the enumeration values.");
        RuleFor(filter => filter.Type).MustBeValidEnumValueOrDefault().WithMessage("Type must be valid for the enumeration values.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
