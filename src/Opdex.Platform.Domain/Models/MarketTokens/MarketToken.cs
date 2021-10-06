using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.MarketTokens
{
    public class MarketToken : BlockAudit
    {
        public MarketToken(ulong marketId, ulong tokenId, ulong createdBlock)
            : base(createdBlock)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
            }

            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than 0.");
            }

            MarketId = marketId;
            TokenId = tokenId;
        }

        public MarketToken(ulong id, ulong marketId, ulong tokenId, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
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
