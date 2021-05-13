using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Domain.Models.ODX
{
    public class Vault
    {
        public Vault(string address, long tokenId, string owner)
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
            
            Address = address;
            TokenId = tokenId;
            Owner = owner;
        }
        
        public Vault(long id, string address, long tokenId, string owner)
        {
            Id = id;
            Address = address;
            TokenId = tokenId;
            Owner = owner;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long TokenId { get; }
        public string Owner { get; private set; }

        public void SetOwner(VaultOwnerChangeLog log)
        {
            Owner = log.To;
        }
    }
}