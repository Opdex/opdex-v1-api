using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;

public class SelectMarketPermissionQuery : FindQuery<MarketPermission>
{
    public SelectMarketPermissionQuery(ulong marketId, Address address, MarketPermissionType permission, bool findOrThrow = true) : base(findOrThrow)
    {
        if (marketId < 1) throw new ArgumentOutOfRangeException(nameof(marketId), "Id must be greater than zero.");
        if (address == Address.Empty) throw new ArgumentNullException(nameof(address), "Address must be set.");
        if (!permission.IsValid()) throw new ArgumentOutOfRangeException(nameof(permission), "Permission must be valid.");
        MarketId = marketId;
        Address = address;
        Permission = permission;
    }

    public ulong MarketId { get; }
    public Address Address { get; }
    public MarketPermissionType Permission { get; }
}