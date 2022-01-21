using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Common.Models;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions;

public class CreateNotifyUserOfTransactionBroadcastCommandHandler : IRequestHandler<CreateNotifyUserOfTransactionBroadcastCommand, bool>
{
    private readonly IMediator _mediator;

    public CreateNotifyUserOfTransactionBroadcastCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<bool> Handle(CreateNotifyUserOfTransactionBroadcastCommand request, CancellationToken cancellationToken)
    {
        // if transaction is already indexed, do not notify
        var transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(request.TransactionHash, false), cancellationToken);
        if (transaction is not null) return false;

        // find transaction on node, this isn't _really_ the sender, is the change output address
        var sender = await _mediator.Send(new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(request.TransactionHash), cancellationToken);

        // If there is a found "sender" above, it must match the provided sender through the callback
        if (sender != Address.Empty && sender != request.Sender) return false;

        // If a "sender" wasn't found, just return out the sender sent
        sender = sender == Address.Empty ? request.Sender : sender;

        await _mediator.Send(new MakeNotifyUserOfTransactionBroadcastCommand(sender, request.TransactionHash), cancellationToken);
        return true;
    }
}
