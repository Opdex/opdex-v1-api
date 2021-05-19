using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Domain.Models.ODX
{
    public class Vault
    {
        public Vault(string address, long tokenId, string owner, ulong createdBlock, ulong modifiedBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            if (!owner.HasValue())
            {
                Owner = owner;
            }
            
            if (createdBlock < 1)
            {
                throw new ArgumentNullException(nameof(createdBlock));
            }
            
            if (modifiedBlock < 1)
            {
                throw new ArgumentNullException(nameof(modifiedBlock));
            }
            
            Address = address;
            TokenId = tokenId;
            Owner = owner;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public Vault(long id, string address, long tokenId, string owner, ulong createdBlock, ulong modifiedBlock)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            Owner = owner;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; }
        public string Owner { get; private set; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; }

        public void SetOwner(ChangeVaultOwnerLog log)
        {
            Owner = log.To;
        }
    }
}