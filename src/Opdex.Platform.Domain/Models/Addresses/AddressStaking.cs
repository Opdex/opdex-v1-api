using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressStaking
    {
        public AddressStaking(long liquidityPoolId, string owner, string weight, ulong createdBlock, ulong modifiedBlock) 
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }
            
            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (!weight.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(weight));
            }

            if (createdBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(createdBlock));
            }

            if (modifiedBlock < 1)
            {
                throw new ArgumentNullException(nameof(modifiedBlock));
            }
            
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Weight = weight;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public AddressStaking(long id, long liquidityPoolId, string owner, string weight, ulong createdBlock, ulong modifiedBlock) 
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
            Weight = weight;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public long Id { get; }
        public long LiquidityPoolId { get; }
        public string Owner { get; }
        public string Weight { get; private set; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; private set; }

        public void SetWeight(StartStakingLog log, ulong block)
        {
            Weight = log.Amount;
            SetModifiedBlock(block);
        }
        
        public void SetWeight(StopStakingLog log, ulong block)
        {
            Weight = log.Amount;
            SetModifiedBlock(block);
        }
        
        private void SetModifiedBlock(ulong block)
        {
            if (block < CreatedBlock)
            {
                throw new ArgumentOutOfRangeException(nameof(block));
            }
            
            ModifiedBlock = block;
        }
    }
}