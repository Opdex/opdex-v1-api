using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.MarketTokens
{
    /// <summary>
    /// Retrieves snapshot data from a market for a given token.
    /// </summary>
    public class GetMarketTokenSnapshotsWithFilterQuery : IRequest<TokenSnapshotsDto>
    {
        /// <summary>
        /// Creates a request to retrieve snapshot data from a market for a given token.
        /// </summary>
        /// <param name="token">The address of the token.</param>
        /// <param name="market">The address of the market.</param>
        /// <param name="cursor">The snapshot cursor filter.</param>
        public GetMarketTokenSnapshotsWithFilterQuery(Address market, Address token, SnapshotCursor cursor)
        {
            if (market == Address.Empty) throw new ArgumentNullException(nameof(market));
            if (token == Address.Empty) throw new ArgumentNullException(nameof(token));
            if (cursor is null) throw new ArgumentNullException(nameof(cursor));

            Market = market;
            Token = token;
            Cursor = cursor;
        }

        public Address Market { get; }
        public Address Token { get; }
        public SnapshotCursor Cursor { get; }
    }
}
