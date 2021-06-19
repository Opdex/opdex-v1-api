using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Domain.Models
{
    public class Deployer : BlockAudit
    {
        public Deployer(string address, string owner, ulong createdBlock) : base(createdBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), "Address must be set.");
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
            }

            Address = address;
            Owner = owner;
        }

        public Deployer(long id, string address, string owner, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            Owner = owner;
        }

        public long Id { get; }
        public string Address { get; }
        public string Owner { get; private set; }

        public void SetOwner(ChangeDeployerOwnerLog log, ulong blockHeight)
        {
            Owner = log.To;
            SetModifiedBlock(blockHeight);
        }
    }
}