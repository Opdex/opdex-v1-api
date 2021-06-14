using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectMarketPermissionQuery : FindQuery<MarketPermission>
    {
        public SelectMarketPermissionQuery(long marketId, string address, Permissions permission, bool findOrThrow) : base(findOrThrow)
        {
            MarketId = marketId;
            Address = address;
            Permission = permission;
        }

        public long MarketId { get; }
        public string Address { get; }
        public Permissions Permission { get; }
    }
}