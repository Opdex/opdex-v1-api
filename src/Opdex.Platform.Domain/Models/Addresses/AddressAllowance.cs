using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressAllowance
    {
        public AddressAllowance(long tokenId, long liquidityPoolId, string owner, string spender, string allowance, ulong createdBlock, ulong modifiedBlock)
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

            if (createdBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(createdBlock));
            }

            if (modifiedBlock < 1)
            {
                throw new ArgumentNullException(nameof(modifiedBlock));
            }
            
            TokenId = tokenId;
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Spender = spender;
            Allowance = allowance;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public AddressAllowance(long id, long tokenId, long liquidityPoolId, string owner, string spender, string allowance, ulong createdBlock, ulong modifiedBlock)
        {
            Id = id;
            TokenId = tokenId;
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Spender = spender;
            Allowance = allowance;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        public long Id { get; }
        public long TokenId { get; }
        public long LiquidityPoolId { get; }
        public string Owner { get; }
        public string Spender { get; }
        public string Allowance { get; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; }
    }
}