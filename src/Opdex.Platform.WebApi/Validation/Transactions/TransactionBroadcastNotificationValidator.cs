using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Transactions;

namespace Opdex.Platform.WebApi.Validation.Transactions;

public class TransactionBroadcastNotificationValidator : AbstractValidator<TransactionBroadcastNotificationRequest>
{
    public TransactionBroadcastNotificationValidator()
    {
        RuleFor(request => request.WalletAddress)
            .MustBeNetworkAddress().WithMessage("Wallet address must be a valid network address.");
    }
}
