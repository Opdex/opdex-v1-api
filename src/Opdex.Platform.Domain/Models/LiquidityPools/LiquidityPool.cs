using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.LiquidityPools
{
    public class LiquidityPool : BlockAudit
    {
        public LiquidityPool(Address address, long srcTokenId, long lptTokenId, long marketId, ulong createdBlock) : base(createdBlock)
        {
            if (address == Address.Empty)
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

        // Todo: This shouldn't exist
        public LiquidityPool(Address address, Address tokenAddress, ulong createdBlock) : base(createdBlock)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), $"{nameof(address)} must be provided");
            }

            if (tokenAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(tokenAddress), $"{nameof(tokenAddress)} must be provided");
            }

            Address = address;
            TokenAddress = tokenAddress;
        }

        public LiquidityPool(long id, Address address, long srcTokenId, long lptTokenId, long marketId, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            SrcTokenId = srcTokenId;
            LpTokenId = lptTokenId;
            MarketId = marketId;
        }

        public long Id { get; }
        public Address Address { get; }
        public long SrcTokenId { get; }
        public long LpTokenId { get; }
        public long MarketId { get; }

        // Todo: Rip this out
        public Address TokenAddress { get; }
    }
}
