using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetTokensWithFilterQuery : IRequest<IEnumerable<TokenDto>>
    {
        public GetTokensWithFilterQuery(Address marketAddress, bool? lpToken, uint skip, uint take, string sortBy,
                                                string orderBy, IEnumerable<Address> tokens)
        {
            if (marketAddress == Address.Empty)
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

        public Address MarketAddress { get; }
        public bool? LpToken { get; }
        public uint Skip { get; }
        public uint Take { get; }
        public string SortBy { get; }
        public string OrderBy { get; }
        public IEnumerable<Address> Tokens { get; }
    }
}
