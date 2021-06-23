using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Domain.Models.ODX
{
    public class Vault : BlockAudit
    {
        public Vault(string address, long tokenId, string owner, ulong genesis, ulong createdBlock) : base(createdBlock)
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

            Address = address;
            TokenId = tokenId;
            Owner = owner;
            Genesis = genesis;
        }

        public Vault(long id, string address, long tokenId, string owner, ulong genesis, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            Owner = owner;
            Genesis = genesis;
        }

        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; }
        public string Owner { get; private set; }
        public ulong Genesis { get; }

        public void SetOwner(ChangeVaultOwnerLog log, ulong block)
        {
            Owner = log.To;
            SetModifiedBlock(block);
        }
    }
}