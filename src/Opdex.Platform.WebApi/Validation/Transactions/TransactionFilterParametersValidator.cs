using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.WebApi.Models.Requests.Transactions;

namespace Opdex.Platform.WebApi.Validation.Transactions
{
    public class TransactionFilterParametersValidator : AbstractCursorValidator<TransactionFilterParameters, TransactionsCursor>
    {
        public TransactionFilterParametersValidator()
        {
            RuleFor(filter => filter.Wallet).MustBeNetworkAddressOrEmpty();
            RuleForEach(filter => filter.Contracts).MustBeNetworkAddress();
            RuleForEach(filter => filter.EventTypes).MustBeValidEnumValue();
            RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
        }
    }
}
