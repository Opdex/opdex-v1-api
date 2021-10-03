using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveTokensWithFilterQuery : IRequest<IEnumerable<Token>>
    {
        public RetrieveTokensWithFilterQuery(ulong marketId, bool? lpToken = null, uint skip = 0, uint take = 0, string sortBy = null,
                                             string orderBy = null, IEnumerable<Address> tokens = null)
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

        public ulong MarketId { get; }
        public bool? LpToken { get; }
        public uint Skip { get; }
        public uint Take { get; }
        public string SortBy { get; }
        public string OrderBy { get; }
        public IEnumerable<Address> Tokens { get; }
    }
}
