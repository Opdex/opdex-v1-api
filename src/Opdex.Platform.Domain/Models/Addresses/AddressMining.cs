using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressMining
    {
        public AddressMining(long miningPoolId, string owner, string balance, ulong createdBlock, ulong modifiedBlock) 
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

            if (createdBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(createdBlock));
            }

            if (modifiedBlock < 1)
            {
                throw new ArgumentNullException(nameof(modifiedBlock));
            }
            
            MiningPoolId = miningPoolId;
            Owner = owner;
            Balance = balance;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public AddressMining(long id, long miningPoolId, string owner, string balance, ulong createdBlock, ulong modifiedBlock) 
        {
            Id = id;
            MiningPoolId = miningPoolId;
            Owner = owner;
            Balance = balance;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public long Id { get; }
        public long MiningPoolId { get; }
        public string Owner { get; }
        public string Balance { get; private set; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; private set; }
        
        public void SetBalance(StartMiningLog log, ulong block)
        {
            Balance = log.Amount;
            SetModifiedBlock(block);
        }
        
        public void SetBalance(StopMiningLog log, ulong block)
        {
            Balance = log.Amount;
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