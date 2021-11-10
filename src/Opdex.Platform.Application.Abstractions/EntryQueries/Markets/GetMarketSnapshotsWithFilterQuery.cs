using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Markets
{
    public class GetMarketSnapshotsWithFilterQuery : IRequest<MarketSnapshotsDto>
    {
        /// <summary>
        /// Creates a request to retrieve snapshot data for a given liquidity pool.
        /// </summary>
        /// <param name="market">The address of the market.</param>
        /// <param name="cursor">The snapshot cursor filter.</param>
        public GetMarketSnapshotsWithFilterQuery(Address market, SnapshotCursor cursor)
        {
            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market), "Liquidity pool address must not be empty.");
            }

            Market = market;
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
        }

        public Address Market { get; }
        public SnapshotCursor Cursor { get; }
    }
}
