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
            MarketId = marketId;
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Tokens cursor must be provided.");
        }

        public ulong MarketId { get; }

        public TokensCursor Cursor { get; }
    }
}
