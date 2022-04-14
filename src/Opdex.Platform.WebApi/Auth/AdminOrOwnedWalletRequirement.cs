using Microsoft.AspNetCore.Authorization;

namespace Opdex.Platform.WebApi.Auth;

/// <summary>
/// Requirement for validating indexing operations are being done against the users wallet address.
/// </summary>
/// <remarks>Should only be applied to /wallets/{address} endpoints</remarks>
public class AdminOrOwnedWalletRequirement : IAuthorizationRequirement
{
    public const string Name = "AdminOrOwnedWallet";
}
