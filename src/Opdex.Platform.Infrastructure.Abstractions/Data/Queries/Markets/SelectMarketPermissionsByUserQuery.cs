using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectMarketPermissionsByUserQuery : IRequest<IEnumerable<Permissions>>
    {
        public SelectMarketPermissionsByUserQuery(long marketId, string user)
        {
            if (marketId < 1) throw new ArgumentOutOfRangeException(nameof(marketId), "Id must be greater than zero.");
            if (!user.HasValue()) throw new ArgumentNullException(nameof(user), "User address must be set.");
            MarketId = marketId;
            User = user;
        }

        public long MarketId { get; }
        public string User { get; }
    }
}