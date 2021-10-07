using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class TokenSummary : BlockAudit
    {
        public TokenSummary(ulong marketId, ulong tokenId, ulong createdBlock) : base(createdBlock)
        {
            if (marketId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than zero.");
            }

            if (tokenId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than zero.");
            }

            MarketId = marketId;
            TokenId = tokenId;
        }

        public TokenSummary(ulong id, ulong marketId, ulong tokenId, decimal dailyChangeUsd, decimal priceUsd, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            MarketId = marketId;
            TokenId = tokenId;
            DailyChangeUsd = dailyChangeUsd;
            PriceUsd = priceUsd;
        }

        public ulong Id { get; }
        public ulong MarketId { get; }
        public ulong TokenId { get; }
        public decimal DailyChangeUsd { get; private set; }
        public decimal PriceUsd { get; private set; }

        public void Update(TokenSnapshot snapshot, ulong blockHeight)
        {
            var openPrice = snapshot.Price.Open;
            var closePrice = snapshot.Price.Close;

            DailyChangeUsd = closePrice.PercentChange(openPrice);
            PriceUsd = snapshot.Price.Close;
            SetModifiedBlock(blockHeight);
        }
    }


}
