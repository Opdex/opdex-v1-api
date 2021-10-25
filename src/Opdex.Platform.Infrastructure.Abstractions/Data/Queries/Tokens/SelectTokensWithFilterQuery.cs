using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectTokensWithFilterQuery : IRequest<IEnumerable<Token>>
    {
        public SelectTokensWithFilterQuery(ulong marketId, TokensCursor cursor)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }

            MarketId = marketId;
            Cursor = cursor;
        }

        public ulong MarketId { get; }

        public TokensCursor Cursor { get; }
    }
}
