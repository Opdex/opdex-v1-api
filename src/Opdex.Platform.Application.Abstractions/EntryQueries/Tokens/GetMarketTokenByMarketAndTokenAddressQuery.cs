using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    /// <summary>
    /// Get a market token by its market contract address and token contract address.
    /// </summary>
    public class GetMarketTokenByMarketAndTokenAddressQuery : IRequest<MarketTokenDto>
    {
        /// <summary>
        /// Constructor to create a get market token by market and token address query.
        /// </summary>
        /// <param name="market">The contract address of the market.</param>
        /// <param name="token">The contract address of the token in the market.</param>
        public GetMarketTokenByMarketAndTokenAddressQuery(Address market, Address token)
        {
            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market), "Market address must be provided.");
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
