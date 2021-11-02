using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.LiquidityPools
{
    public class LiquidityPool : BlockAudit
    {
        public LiquidityPool(Address address, string name, ulong srcTokenId, ulong lpTokenId, ulong marketId, ulong createdBlock) : base(createdBlock)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Liquidity pool address must be provided");
            }

            if (!name.HasValue())
            {
                throw new ArgumentNullException(nameof(name), "Name must be provided.");
            }

            if (srcTokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(srcTokenId), "SRC token id must be greater than zero.");
            }

            if (lpTokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lpTokenId), "Liquidity pool token id must be greater than zero.");
            }

            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than zero.");
            }

            Name = name;
            Address = address;
            SrcTokenId = srcTokenId;
            LpTokenId = lpTokenId;
            MarketId = marketId;
        }

        public LiquidityPool(ulong id, Address address, string name, ulong srcTokenId, ulong lpTokenId, ulong marketId, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Name = name;
            Address = address;
            SrcTokenId = srcTokenId;
            LpTokenId = lpTokenId;
            MarketId = marketId;
        }

        public ulong Id { get; }
        public string Name { get; }
        public Address Address { get; }
        public ulong SrcTokenId { get; }
        public ulong LpTokenId { get; }
        public ulong MarketId { get; }
    }
}
