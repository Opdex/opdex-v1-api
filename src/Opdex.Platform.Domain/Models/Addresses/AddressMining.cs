using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressMining : BlockAudit
    {
        public AddressMining(long miningPoolId, string owner, string balance, ulong createdBlock) : base(createdBlock)
        {
            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId));
            }
            
            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (!balance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(balance));
            }

            MiningPoolId = miningPoolId;
            Owner = owner;
            Balance = balance;
        }
        
        public AddressMining(long id, long miningPoolId, string owner, string balance, ulong createdBlock, ulong modifiedBlock) 
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            MiningPoolId = miningPoolId;
            Owner = owner;
            Balance = balance;
        }
        
        public long Id { get; }
        public long MiningPoolId { get; }
        public string Owner { get; }
        public string Balance { get; private set; }
        
        public void SetBalance(StartMiningLog log, ulong block)
        {
            Balance = log.Amount;
            SetModifiedBlock(block);
        }
        
        public void ResetBalance(StopMiningLog log, ulong block)
        {
            if (!log.Amount.Equals(Balance) || Id == 0)
            {
                throw new Exception("Unable to reset mining balance");
            }
            
            Balance = "0";
            SetModifiedBlock(block);
        }
    }
}