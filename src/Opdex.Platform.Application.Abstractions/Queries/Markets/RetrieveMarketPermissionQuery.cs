using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveMarketPermissionQuery : FindQuery<MarketPermission>
    {
        public RetrieveMarketPermissionQuery(long marketId, string address, Permissions permission, bool findOrThrow = true) : base(findOrThrow)
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