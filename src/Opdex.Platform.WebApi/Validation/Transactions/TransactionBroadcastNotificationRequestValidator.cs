using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Transactions;

namespace Opdex.Platform.WebApi.Validation.Transactions;

public class TransactionBroadcastNotificationRequestValidator : AbstractValidator<TransactionBroadcastNotificationRequest>
{
    public TransactionBroadcastNotificationRequestValidator()
    {
        RuleFor(request => request.WalletAddress).MustBeNetworkAddress();
        RuleFor(request => request.TransactionHash).NotNull();
    }
}