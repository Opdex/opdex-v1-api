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
        var existsInMempool = await _mediator.Send(new RetrieveCirrusExistsInMempoolQuery(request.TransactionHash), cancellationToken);
        if (!existsInMempool) return false;
        
        var sender = await _mediator.Send(new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(request.TransactionHash), cancellationToken);
        if (sender == Address.Empty) return false;

        await _mediator.Send(new MakeNotifyUserOfTransactionBroadcastCommand(sender, request.TransactionHash), cancellationToken);
        return true;
    }
}
