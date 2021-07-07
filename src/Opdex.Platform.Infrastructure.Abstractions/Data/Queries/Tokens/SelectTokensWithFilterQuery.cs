using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectTokensWithFilterQuery : IRequest<IEnumerable<Token>>
    {
        public SelectTokensWithFilterQuery(long marketId, bool? lpToken = null, uint skip = 0, uint take = 0, string sortBy = null,
                                           string orderBy = null, IEnumerable<string> tokens = null)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }

            MarketId = marketId;
            LpToken = lpToken;
            Skip = skip;
            Take = take;
            SortBy = sortBy;
            OrderBy = orderBy;
            Tokens = tokens;
        }

        public long MarketId { get; }
        public bool? LpToken { get; }
        public uint Skip { get; }
        public uint Take { get; }
        public string SortBy { get; }
        public string OrderBy { get; }
        public IEnumerable<string> Tokens { get; }
    }
}