using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Transactions;

public class MakeNotifyUserOfTransactionBroadcastCommandHandler : AsyncRequestHandler<MakeNotifyUserOfTransactionBroadcastCommand>
{
    private readonly IMediator _mediator;

    public MakeNotifyUserOfTransactionBroadcastCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override async Task Handle(MakeNotifyUserOfTransactionBroadcastCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new NotifyUserOfBroadcastTransactionCommand(request.User, request.TransactionHash), cancellationToken);
    }
}
