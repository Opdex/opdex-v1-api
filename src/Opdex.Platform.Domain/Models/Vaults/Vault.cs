using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

namespace Opdex.Platform.Domain.Models.ODX
{
    public class Vault : BlockAudit
    {
        public Vault(Address address, long tokenId, Address owner, ulong genesis, UInt256 unassignedSupply, ulong createdBlock) : base(createdBlock)
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

            if (genesis < 1)
            {
                throw new ArgumentNullException(nameof(genesis), "Genesis must be greater than 0.");
            }

            Address = address;
            TokenId = tokenId;
            Owner = owner;
            Genesis = genesis;
            UnassignedSupply = unassignedSupply;
        }

        public Vault(long id, Address address, long tokenId, Address owner, ulong genesis, UInt256 unassignedSupply, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            Owner = owner;
            Genesis = genesis;
            UnassignedSupply = unassignedSupply;
        }

        public long Id { get; }
        public Address Address { get; }
        public long TokenId { get; }
        public Address Owner { get; private set; }
        public ulong Genesis { get; }
        public UInt256 UnassignedSupply { get; private set; }

        public void SetOwner(ClaimPendingVaultOwnershipLog log, ulong block)
        {
            Owner = log.To;
            SetModifiedBlock(block);
        }

        public void SetUnassignedSupply(UInt256 unassignedSupply, ulong block)
        {
            UnassignedSupply = unassignedSupply;
            SetModifiedBlock(block);
        }
    }
}
