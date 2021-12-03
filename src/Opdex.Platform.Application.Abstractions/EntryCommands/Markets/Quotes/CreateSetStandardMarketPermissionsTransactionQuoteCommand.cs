using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;

/// <summary>
/// Quote a transaction to set user permissions within a standard market.
/// </summary>
public class CreateSetStandardMarketPermissionsTransactionQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a command to set user permissions within a standard market.
    /// </summary>
    /// <param name="market">The address of the market.</param>
    /// <param name="authority">The address which has the authority to set user permissions.</param>
    /// <param name="user">The address of the user to assign permissions.</param>
    /// <param name="permission">The permission to update.</param>
    /// <param name="authorize">Whether to authorize the user this market permission.</param>
    /// <returns></returns>
    public CreateSetStandardMarketPermissionsTransactionQuoteCommand(Address market, Address authority, Address user, MarketPermissionType permission, bool authorize) : base(authority)
    {
        Market = market != Address.Empty ? market : throw new ArgumentNullException(nameof(market), "Market address must be set.");
        User = user != Address.Empty ? user : throw new ArgumentNullException(nameof(user), "User address must be set.");
        Permission = permission.IsValid() ? permission : throw new ArgumentNullException(nameof(permission), "Permission must be valid.");
        Authorize = authorize;
    }

    public Address Market { get; }
    public Address User { get; }
    public MarketPermissionType Permission { get; }
    public bool Authorize { get; }
}