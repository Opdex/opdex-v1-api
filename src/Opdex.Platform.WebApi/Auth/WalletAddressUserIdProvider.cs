using Microsoft.AspNetCore.SignalR;

namespace Opdex.Platform.WebApi.Auth;

public class WalletAddressUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirst("wallet")?.Value;
    }
}
