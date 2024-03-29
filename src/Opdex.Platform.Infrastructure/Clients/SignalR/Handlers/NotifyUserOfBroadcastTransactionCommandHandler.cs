using MediatR;
using Microsoft.AspNetCore.SignalR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.SignalR.Handlers;

public class NotifyUserOfBroadcastTransactionCommandHandler : AsyncRequestHandler<NotifyUserOfBroadcastTransactionCommand>
{
    private readonly IHubContext<PlatformHub, IPlatformClient> _hubContext;

    public NotifyUserOfBroadcastTransactionCommandHandler(IHubContext<PlatformHub, IPlatformClient> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task Handle(NotifyUserOfBroadcastTransactionCommand request, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.User(request.User.ToString()).OnTransactionBroadcast(request.TxHash.ToString());
    }
}