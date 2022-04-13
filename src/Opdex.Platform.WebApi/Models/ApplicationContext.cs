using Microsoft.AspNetCore.Http;
using Opdex.Platform.Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Opdex.Platform.WebApi.Models;

public class ApplicationContext : IApplicationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Address Wallet => GetWallet();

    private Address GetWallet()
    {
        var subject = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);

        return new Address(subject?.Value);
    }
}
