using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveMarketPermissionQuery : FindQuery<MarketPermission>
    {
        public RetrieveMarketPermissionQuery(long marketId, Address address, Permissions permission, bool findOrThrow = true) : base(findOrThrow)
        {
            if (marketId < 1) throw new ArgumentOutOfRangeException(nameof(marketId), "Id must be greater than zero.");
            if (address == Address.Empty) throw new ArgumentNullException(nameof(address), "Address must be set.");
            if (permission == Permissions.Unknown) throw new ArgumentNullException(nameof(permission), "Permission must be set.");
            MarketId = marketId;
            Address = address;
            Permission = permission;
        }

        public long MarketId { get; }
        public Address Address { get; }
        public Permissions Permission { get; }
    }
}
