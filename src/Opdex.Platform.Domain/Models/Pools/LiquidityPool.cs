using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Pools
{
    public class LiquidityPool : BlockAudit
    {
        public LiquidityPool(string address, long srcTokenId, long lptTokenId, long marketId, ulong createdBlock) : base(createdBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }

            if (srcTokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(srcTokenId), $"{nameof(srcTokenId)} must be greater than 0.");
            }

            if (lptTokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lptTokenId), $"{nameof(lptTokenId)} must be greater than 0.");
            }

            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            Address = address;
            SrcTokenId = srcTokenId;
            LpTokenId = lptTokenId;
            MarketId = marketId;
        }

        // Todo: this probably shouldn't exist
        public LiquidityPool(string address, string tokenAddress, ulong createdBlock) : base(createdBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }

            if (!tokenAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(tokenAddress), $"{nameof(tokenAddress)} must be provided");
            }

            Address = address;
            TokenAddress = tokenAddress;
        }

        public LiquidityPool(long id, string address, long srcTokenId, long lptTokenId, long marketId, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            SrcTokenId = srcTokenId;
            LpTokenId = lptTokenId;
            MarketId = marketId;
        }

        public long Id { get; }
        public string Address { get; }
        public long SrcTokenId { get; private set; }
        public long LpTokenId { get; private set; }
        public long MarketId { get; private set; }
        public string TokenAddress { get; }

        public void SetSrcTokenId(long tokenId)
        {
            if (SrcTokenId == 0 && tokenId > 0)
            {
                SrcTokenId = tokenId;
            }
        }

        public void SetLpTokenId(long tokenId)
        {
            if (LpTokenId == 0 && tokenId > 0)
            {
                LpTokenId = tokenId;
            }
        }

        public void SetMarketId(long marketId)
        {
            if (MarketId == 0 && marketId > 0)
            {
                MarketId = marketId;
            }
        }
    }
}