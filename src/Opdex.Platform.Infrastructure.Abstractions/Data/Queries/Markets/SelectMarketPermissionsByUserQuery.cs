using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectMarketPermissionsByUserQuery : IRequest<IEnumerable<Permissions>>
    {
        public SelectMarketPermissionsByUserQuery(long marketId, string user)
        {
            MarketId = marketId;
            User = user;
        }

        public long MarketId { get; }
        public string User { get; }
    }
}