using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Domain.Models
{
    public class Deployer : BlockAudit
    {
        public Deployer(Address address, Address owner, ulong createdBlock) : base(createdBlock)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Address must be set.");
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
            }

            Address = address;
            Owner = owner;
        }

        public Deployer(long id, Address address, Address owner, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            Owner = owner;
        }

        public long Id { get; }
        public Address Address { get; }
        public Address Owner { get; private set; }

        public void SetOwner(ClaimPendingDeployerOwnershipLog log, ulong blockHeight)
        {
            Owner = log.To;
            SetModifiedBlock(blockHeight);
        }
    }
}
