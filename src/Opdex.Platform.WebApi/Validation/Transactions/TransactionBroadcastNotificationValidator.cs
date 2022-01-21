using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Transactions;

namespace Opdex.Platform.WebApi.Validation.Transactions;

public class TransactionBroadcastNotificationValidator : AbstractValidator<TransactionBroadcastNotificationRequest>
{
    public TransactionBroadcastNotificationValidator()
    {
        RuleFor(request => request.TransactionHash)
            .NotEmpty().WithMessage("Transaction hash must not be empty.")
            .NotEqual(new Sha256()).WithMessage("Transaction hash must be valid.");

        RuleFor(request => request.PublicKey)
            .MustBeNetworkAddress().WithMessage("Public key must be a valid network address.");
    }
}
