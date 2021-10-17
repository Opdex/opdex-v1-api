using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.WebApi.Models.Requests.Wallets;

namespace Opdex.Platform.WebApi.Validation.Wallets
{
    public class AddressBalanceFilterParametersValidator : AbstractCursorValidator<AddressBalanceFilterParameters, AddressBalancesCursor>
    {
        public AddressBalanceFilterParametersValidator()
        {
            RuleForEach(filter => filter.Tokens).MustBeNetworkAddress();
            RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
        }
    }
}
