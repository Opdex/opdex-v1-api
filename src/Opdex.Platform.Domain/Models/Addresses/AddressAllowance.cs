using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressAllowance : BlockAudit
    {
        public AddressAllowance(long tokenId, long liquidityPoolId, string owner, string spender, string allowance, ulong createdBlock)
            : base(createdBlock)
        {
            if (tokenId < 1 && liquidityPoolId < 1)
            {
                throw new ArgumentException("Either liquidityPoolId or tokenId must be greater than 0.");
            }

            if (tokenId >= 1 && liquidityPoolId >= 1)
            {
                throw new ArgumentException("Only liquidityPoolId or tokenId can be greater than 0.");
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (!spender.HasValue())
            {
                throw new ArgumentNullException(nameof(spender), "Spender must be set.");
            }

            if (!allowance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(allowance), "Allowance must only contain numeric digits.");
            }

            TokenId = tokenId;
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Spender = spender;
            Allowance = allowance;
        }

        public AddressAllowance(long id, long tokenId, long liquidityPoolId, string owner, string spender, string allowance,
            ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            TokenId = tokenId;
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Spender = spender;
            Allowance = allowance;
        }

        public long Id { get; }
        public long TokenId { get; }
        public long LiquidityPoolId { get; }
        public string Owner { get; }
        public string Spender { get; }
        public string Allowance { get; }
    }
}