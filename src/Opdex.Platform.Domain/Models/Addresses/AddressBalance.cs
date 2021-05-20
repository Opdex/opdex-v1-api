using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressBalance : BlockAudit
    {
        public AddressBalance(long tokenId, long liquidityPoolId, string owner, string balance, ulong createdBlock) : base(createdBlock)
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

            if (!balance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(balance));
            }
            
            TokenId = tokenId;
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Balance = balance;
        }
        
        public AddressBalance(long id, long tokenId, long liquidityPoolId, string owner, string balance, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            TokenId = tokenId;
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Balance = balance;
        }
        
        public long Id { get; }
        public long TokenId { get; }
        public long LiquidityPoolId { get; }
        public string Owner { get; }
        public string Balance { get; private set; }

        public void SetBalance(string balance, ulong blockHeight)
        {
            if (!balance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(balance));
            }

            Balance = balance;
            SetModifiedBlock(blockHeight);
        }
    }
}