using MediatR;
using Microsoft.AspNetCore.SignalR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.SignalR.Handlers
{
    public class NotifyUserOfSuccessfulAuthenticationCommandHandler : AsyncRequestHandler<NotifyUserOfSuccessfulAuthenticationCommand>
    {
        private readonly IHubContext<PlatformHub, IPlatformClient> _hubContext;

        public NotifyUserOfSuccessfulAuthenticationCommandHandler(IHubContext<PlatformHub, IPlatformClient> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        protected override async Task Handle(NotifyUserOfSuccessfulAuthenticationCommand request, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.Client(request.ConnectionId).OnAuthenticated(request.BearerToken);
        }
    }
}