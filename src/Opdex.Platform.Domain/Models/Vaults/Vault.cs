using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;

namespace Opdex.Platform.Domain.Models.Vaults
{
    public class Vault : BlockAudit
    {
        public Vault(string address, long tokenId, string owner, ulong genesis, string unassignedSupply, ulong createdBlock) : base(createdBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), "Address must be set.");
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
            }

            if (!owner.HasValue())
            {
                Owner = owner;
            }

            if (genesis < 1)
            {
                throw new ArgumentNullException(nameof(genesis), "Genesis must be greater than 0.");
            }

            if (!unassignedSupply.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(unassignedSupply), "Unassigned supply must only contain numeric digits.");
            }

            Address = address;
            TokenId = tokenId;
            Owner = owner;
            Genesis = genesis;
            UnassignedSupply = unassignedSupply;
        }

        public Vault(long id, string address, long tokenId, string owner, ulong genesis, string unassignedSupply, ulong createdBlock, ulong modifiedBlock)
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
        public string Address { get; }
        public long TokenId { get; }
        public string Owner { get; private set; }
        public ulong Genesis { get; }
        public string UnassignedSupply { get; private set; }

        public void SetOwner(ClaimPendingVaultOwnershipLog log, ulong block)
        {
            Owner = log.To;
            SetModifiedBlock(block);
        }

        public void SetUnassignedSupply(string unassignedSupply, ulong block)
        {
            if (!unassignedSupply.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(unassignedSupply), "Unassigned supply must only contain numeric digits.");
            }

            UnassignedSupply = unassignedSupply;
            SetModifiedBlock(block);
        }
    }
}
