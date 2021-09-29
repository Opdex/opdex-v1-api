using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

namespace Opdex.Platform.Domain.Models.Vaults
{
    public class Vault : BlockAudit
    {
        public Vault(Address address, long tokenId, Address owner, ulong createdBlock) : base(createdBlock)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Address must be set.");
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Owner address must be set.");
            }

            Address = address;
            TokenId = tokenId;
            Owner = owner;
        }

        public Vault(long id, Address address, long tokenId, Address pendingOwner, Address owner, ulong genesis, UInt256 unassignedSupply, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            PendingOwner = pendingOwner;
            Owner = owner;
            Genesis = genesis;
            UnassignedSupply = unassignedSupply;
        }

        public long Id { get; }
        public Address Address { get; }
        public long TokenId { get; }
        public Address PendingOwner { get; private set; }
        public Address Owner { get; private set; }
        public ulong Genesis { get; private set; }
        public UInt256 UnassignedSupply { get; private set; }

        public void SetPendingOwnership(SetPendingVaultOwnershipLog log, ulong block)
        {
            if (log is null) throw new ArgumentNullException(nameof(log));

            PendingOwner = log.To;
            SetModifiedBlock(block);
        }

        public void SetOwnershipClaimed(ClaimPendingVaultOwnershipLog log, ulong block)
        {
            if (log is null) throw new ArgumentNullException(nameof(log));

            PendingOwner = Address.Empty;
            Owner = log.To;
            SetModifiedBlock(block);
        }

        public void Update(VaultContractSummary summary)
        {
            if (summary is null) throw new ArgumentNullException(nameof(summary));

            if (summary.PendingOwner.HasValue) PendingOwner = summary.PendingOwner.Value;
            if (summary.Owner.HasValue) Owner = summary.Owner.Value;
            if (summary.Genesis.HasValue) Genesis = summary.Genesis.Value;
            if (summary.UnassignedSupply.HasValue) UnassignedSupply = summary.UnassignedSupply.Value;
            SetModifiedBlock(summary.BlockHeight);
        }
    }
}
