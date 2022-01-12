using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.WebApi.Models.Requests.Wallets;

namespace Opdex.Platform.WebApi.Validation.Wallets;

public class AddressBalanceFilterParametersValidator : AbstractCursorValidator<AddressBalanceFilterParameters, AddressBalancesCursor>
{
    public AddressBalanceFilterParametersValidator()
    {
        RuleForEach(filter => filter.Tokens).MustBeNetworkAddress().WithMessage("Token must be valid address.");
        RuleForEach(filter => filter.TokenAttributes).MustBeValidEnumValue().WithMessage("Token attributes must be valid for the enumeration values.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
