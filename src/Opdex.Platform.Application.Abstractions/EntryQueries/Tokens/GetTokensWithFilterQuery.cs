using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetTokensWithFilterQuery : IRequest<IEnumerable<TokenDto>>
    {
        public GetTokensWithFilterQuery(string marketAddress, bool? lpToken, uint skip, uint take, string sortBy,
                                                string orderBy, IEnumerable<string> tokens)
        {
            if (!marketAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(marketAddress));
            }

            MarketAddress = marketAddress;
            LpToken = lpToken;
            Skip = skip;
            Take = take;
            SortBy = sortBy;
            OrderBy = orderBy;
            Tokens = tokens;
        }

        public string MarketAddress { get; }
        public bool? LpToken { get; }
        public uint Skip { get; }
        public uint Take { get; }
        public string SortBy { get; }
        public string OrderBy { get; }
        public IEnumerable<string> Tokens { get; }
    }
}
