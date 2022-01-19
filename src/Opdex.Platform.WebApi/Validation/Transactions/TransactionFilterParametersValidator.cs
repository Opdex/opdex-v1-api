using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.WebApi.Models.Requests.Transactions;

namespace Opdex.Platform.WebApi.Validation.Transactions;

public class TransactionFilterParametersValidator : AbstractCursorValidator<TransactionFilterParameters, TransactionsCursor>
{
    public TransactionFilterParametersValidator()
    {
        RuleFor(filter => filter.Sender).MustBeNetworkAddressOrEmpty().WithMessage("Wallet must be valid address.");
        RuleForEach(filter => filter.Contracts).MustBeNetworkAddress().WithMessage("Contract must be valid address.");
        RuleForEach(filter => filter.EventTypes).MustBeValidEnumValue().WithMessage("Event type must be valid for the enumeration values.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
