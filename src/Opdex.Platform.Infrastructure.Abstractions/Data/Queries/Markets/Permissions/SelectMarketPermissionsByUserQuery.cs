using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;

public class SelectMarketPermissionsByUserQuery : IRequest<IEnumerable<MarketPermissionType>>
{
    public SelectMarketPermissionsByUserQuery(ulong marketId, Address user)
    {
        if (marketId < 1) throw new ArgumentOutOfRangeException(nameof(marketId), "Id must be greater than zero.");
        if (user == Address.Empty) throw new ArgumentNullException(nameof(user), "User address must be set.");
        MarketId = marketId;
        User = user;
    }

    public ulong MarketId { get; }
    public Address User { get; }
}