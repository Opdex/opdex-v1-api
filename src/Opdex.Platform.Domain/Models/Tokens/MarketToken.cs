using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class MarketToken : TokenBase
    {
        public MarketToken(Market market, TokenBase token)
            : base(token.Id, token.Address, token.IsLpt, token.Name, token.Symbol, token.Decimals, token.Sats, token.TotalSupply,
                   token.CreatedBlock, token.ModifiedBlock)
        {
            Market = market ?? throw new ArgumentNullException(nameof(market), "Market must be provided.");
        }

        public Market Market { get; }
    }
}
