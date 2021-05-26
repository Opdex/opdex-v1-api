using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressStaking : BlockAudit
    {
        public AddressStaking(long liquidityPoolId, string owner, string weight, ulong createdBlock) : base(createdBlock)
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

        public void SetWeight(StartStakingLog log, ulong block)
        {
            Weight = log.Amount;
            SetModifiedBlock(block);
        }
        
        public void ResetWeight(StopStakingLog log, ulong block)
        {
            if (!log.Amount.Equals(Weight) || Id == 0)
            {
                throw new Exception("Unable to reset staking weight");
            }
            
            Weight = "0";
            SetModifiedBlock(block);
        }
    }
}