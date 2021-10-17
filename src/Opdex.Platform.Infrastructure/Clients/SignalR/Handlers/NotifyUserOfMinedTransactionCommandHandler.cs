using MediatR;
using Microsoft.AspNetCore.SignalR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.SignalR.Handlers
{
    public class NotifyUserOfMinedTransactionCommandHandler : AsyncRequestHandler<NotifyUserOfMinedTransactionCommand>
    {
        private readonly IHubContext<PlatformHub, IPlatformClient> _hubContext;

        public NotifyUserOfMinedTransactionCommandHandler(IHubContext<PlatformHub, IPlatformClient> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task Handle(NotifyUserOfMinedTransactionCommand request, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.User(request.User.ToString()).OnTransactionMined(request.TxHash);
        }
    }
}
