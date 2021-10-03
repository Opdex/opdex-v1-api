using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using System;

namespace Opdex.Platform.Domain.Models.Deployers
{
    public class Deployer : BlockAudit
    {
        public Deployer(Address address, Address owner, bool isActive, ulong createdBlock) : base(createdBlock)
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
            IsActive = isActive;
        }

        public Deployer(ulong id, Address address, Address pendingOwner, Address owner, bool isActive, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            PendingOwner = pendingOwner;
            Owner = owner;
            IsActive = isActive;
        }

        public ulong Id { get; }
        public Address Address { get; }
        public Address PendingOwner { get; private set; }
        public Address Owner { get; private set; }
        public bool IsActive { get; }

        public void SetPendingOwnership(SetPendingDeployerOwnershipLog log, ulong block)
        {
            if (log is null) throw new ArgumentNullException(nameof(log));

            PendingOwner = log.To;
            SetModifiedBlock(block);
        }

        public void SetOwnershipClaimed(ClaimPendingDeployerOwnershipLog log, ulong blockHeight)
        {
            if (log is null) throw new ArgumentNullException(nameof(log));

            PendingOwner = Address.Empty;
            Owner = log.To;
            SetModifiedBlock(blockHeight);
        }

        public void Update(DeployerContractSummary summary)
        {
            if (summary is null) throw new ArgumentNullException(nameof(summary));

            if (summary.PendingOwner.HasValue) PendingOwner = summary.PendingOwner.Value;
            if (summary.Owner.HasValue) Owner = summary.Owner.Value;
            SetModifiedBlock(summary.BlockHeight);
        }
    }
}
