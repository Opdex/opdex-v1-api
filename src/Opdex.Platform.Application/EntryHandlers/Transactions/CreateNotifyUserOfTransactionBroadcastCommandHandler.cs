using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Common.Models;
using System;
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

        // find transaction on node, this isn't _really_ the sender, is the change output address, can throw not found if the transaction is not found
        var changeAddress = await _mediator.Send(new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(request.TransactionHash), cancellationToken);

        // If a change address is found and it doesn't match the alleged sender, don't notify
        if (changeAddress != Address.Empty && changeAddress != request.Sender) return false;

        // If a change address wasn't found, trust the requests sender, notify anyways
        var senderToNotify = changeAddress == Address.Empty ? request.Sender : changeAddress;

        await _mediator.Send(new MakeNotifyUserOfTransactionBroadcastCommand(senderToNotify, request.TransactionHash), cancellationToken);
        return true;
    }
}
