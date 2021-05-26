using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressAllowance : BlockAudit
    {
        public AddressAllowance(long tokenId, long liquidityPoolId, string owner, string spender, string allowance, ulong createdBlock)
            : base(createdBlock)
        {
            if (tokenId < 1 && liquidityPoolId < 1)
            {
                throw new Exception("Either liquidityPoolId or tokenId must be greater than 0.");
            }

            if (tokenId > 1 && liquidityPoolId > 1)
            {
                throw new Exception("Only liquidityPoolId or tokenId can be greater than 0.");
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }
            
            if (!spender.HasValue())
            {
                throw new ArgumentNullException(nameof(spender));
            }

            if (!allowance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(allowance));
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