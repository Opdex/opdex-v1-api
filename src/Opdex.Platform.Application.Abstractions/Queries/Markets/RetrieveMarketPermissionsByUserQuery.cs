using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveMarketPermissionsByUserQuery : IRequest<IEnumerable<Permissions>>
    {
        public RetrieveMarketPermissionsByUserQuery(long marketId, Address user)
        {
            if (marketId < 1) throw new ArgumentOutOfRangeException(nameof(marketId), "Id must be greater than zero.");
            if (user == Address.Empty) throw new ArgumentNullException(nameof(user), "User address must be set.");
            MarketId = marketId;
            User = user;
        }

        public long MarketId { get; }
        public Address User { get; }
    }
}
