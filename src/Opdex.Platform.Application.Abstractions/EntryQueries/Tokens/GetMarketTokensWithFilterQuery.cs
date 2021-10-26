using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    /// <summary>
    /// Get market tokens with pagination and filtering.
    /// </summary>
    public class GetMarketTokensWithFilterQuery : IRequest<MarketTokensDto>
    {
        /// <summary>
        /// Constructor to build a get market tokens with filter query.
        /// </summary>
        /// <param name="market">The market address to get tokens from within.</param>
        /// <param name="cursor">The cursor used for filtering and pagination.</param>
        public GetMarketTokensWithFilterQuery(Address market, TokensCursor cursor)
        {
            Market = market != Address.Empty ? market : throw new ArgumentNullException(nameof(market), "Market address must be set.");
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Tokens cursor must be set.");
        }

        public Address Market { get; }
        public TokensCursor Cursor { get; }
    }
}
