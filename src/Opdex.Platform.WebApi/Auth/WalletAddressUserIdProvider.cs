using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Opdex.Platform.WebApi.Auth;

public class WalletAddressUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
    }
}
