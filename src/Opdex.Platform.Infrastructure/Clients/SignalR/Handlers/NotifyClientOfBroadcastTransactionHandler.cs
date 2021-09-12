using MediatR;
using Microsoft.AspNetCore.SignalR;
using Opdex.Platform.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.SignalR.Handlers
{
    public class NotifyClientOfBroadcastTransactionHandler : INotificationHandler<TransactionBroadcastNotification>
    {
        private readonly IHubContext<PlatformHub, IPlatformClient> _hubContext;

        public NotifyClientOfBroadcastTransactionHandler(IHubContext<PlatformHub, IPlatformClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(TransactionBroadcastNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.User(notification.User.ToString()).OnTransactionBroadcast(notification.TxHash);
        }
    }
}
