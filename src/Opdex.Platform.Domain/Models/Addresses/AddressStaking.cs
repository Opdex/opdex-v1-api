using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressStaking : BlockAudit
    {
        public AddressStaking(long liquidityPoolId, string owner, string weight, ulong createdBlock) : base(createdBlock)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than 0.");
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (!weight.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must only contain numeric digits.");
            }

            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Weight = weight;
        }

        public AddressStaking(long id, long liquidityPoolId, string owner, string weight, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Weight = weight;
        }

        public long Id { get; }
        public long LiquidityPoolId { get; }
        public string Owner { get; }
        public string Weight { get; private set; }

        public void SetWeight(string weight, ulong block)
        {
            if (!weight.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must only contain numeric digits.");
            }

            Weight = weight;
            SetModifiedBlock(block);
        }
    }
}