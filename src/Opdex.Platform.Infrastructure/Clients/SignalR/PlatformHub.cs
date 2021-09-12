using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Opdex.Platform.Infrastructure.Clients.SignalR
{
    [Authorize]
    public class PlatformHub : Hub<IPlatformClient>
    {
    }
}
