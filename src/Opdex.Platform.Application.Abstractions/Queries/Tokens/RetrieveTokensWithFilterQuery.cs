using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveTokensWithFilterQuery : IRequest<IEnumerable<Token>>
    {
        public RetrieveTokensWithFilterQuery(ulong marketId, TokensCursor cursor)
        {
            MarketId = marketId;
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public ulong MarketId { get; }
        public TokensCursor Cursor { get; }
    }
}
