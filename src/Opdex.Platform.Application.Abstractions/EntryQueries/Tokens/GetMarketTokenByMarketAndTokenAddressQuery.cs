using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetMarketTokenByMarketAndTokenAddressQuery : IRequest<MarketTokenDto>
    {
        public GetMarketTokenByMarketAndTokenAddressQuery(Address market, Address token)
        {
            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market), "Market address must be provided");
            }

            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token), "Token address must be provided.");
            }

            Market = market;
            Token = token;
        }

        public Address Market { get; }
        public Address Token { get; }
    }
}
