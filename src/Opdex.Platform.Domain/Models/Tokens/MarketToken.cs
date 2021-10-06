using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class MarketToken : BaseToken
    {
        public MarketToken(ulong marketId, ulong tokenId, ulong createdBlock)
            : base(createdBlock)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than zero.");
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than zero.");
            }

            MarketId = marketId;
            TokenId = tokenId;
        }

        public MarketToken(ulong id, ulong marketId, ulong tokenId, Address address, bool isLpt, string name, string symbol, int decimals,
                           ulong sats, UInt256 totalSupply, ulong createdBlock, ulong modifiedBlock)
            : base(address, isLpt, name, symbol, decimals, sats, totalSupply, createdBlock, modifiedBlock)
        {
            Id = id;
            MarketId = marketId;
            TokenId = tokenId;
        }

        public ulong Id { get; }
        public ulong MarketId { get; }
        public ulong TokenId { get; }
    }
}
