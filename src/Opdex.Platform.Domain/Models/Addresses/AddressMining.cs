using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressMining : BlockAudit
    {
        public AddressMining(ulong miningPoolId, Address owner, UInt256 balance, ulong createdBlock) : base(createdBlock)
        {
            if (miningPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(miningPoolId), "Mining pool id must be greater than 0.");
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            MiningPoolId = miningPoolId;
            Owner = owner;
            Balance = balance;
        }

        public AddressMining(ulong id, ulong miningPoolId, Address owner, UInt256 balance, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            MiningPoolId = miningPoolId;
            Owner = owner;
            Balance = balance;
        }

        public ulong Id { get; }
        public ulong MiningPoolId { get; }
        public Address Owner { get; }
        public UInt256 Balance { get; private set; }

        public void SetBalance(UInt256 balance, ulong block)
        {
            Balance = balance;
            SetModifiedBlock(block);
        }
    }
}
