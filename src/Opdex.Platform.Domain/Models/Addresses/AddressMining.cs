using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressMining : BlockAudit
    {
        public AddressMining(long miningPoolId, string owner, string balance, ulong createdBlock) : base(createdBlock)
        {
            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining pool id must be greater than 0.");
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (!balance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance must only contain numeric digits.");
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

        public void SetBalance(string balance, ulong block)
        {
            if (!balance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance must only contain numeric digits.");
            }

            Balance = balance;

            SetModifiedBlock(block);
        }
    }
}