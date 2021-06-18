using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveMarketPermissionsByUserQuery : IRequest<IEnumerable<Permissions>>
    {
        public RetrieveMarketPermissionsByUserQuery(long marketId, string user)
        {
            MarketId = marketId;
            User = user;
        }

        public long MarketId { get; }
        public string User { get; }
    }
}