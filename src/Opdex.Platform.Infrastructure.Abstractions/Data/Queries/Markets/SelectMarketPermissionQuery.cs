using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectMarketPermissionQuery : FindQuery<MarketPermission>
    {
        public SelectMarketPermissionQuery(long marketId, string address, Permissions permission, bool findOrThrow = true) : base(findOrThrow)
        {
            if (marketId < 1) throw new ArgumentOutOfRangeException(nameof(marketId), "Id must be greater than zero.");
            if (!address.HasValue()) throw new ArgumentNullException(nameof(address), "Address must be set.");
            if (!permission.IsValid()) throw new ArgumentOutOfRangeException(nameof(permission), "Permission must be valid.");
            MarketId = marketId;
            Address = address;
            Permission = permission;
        }

        public long MarketId { get; }
        public string Address { get; }
        public Permissions Permission { get; }
    }
}
