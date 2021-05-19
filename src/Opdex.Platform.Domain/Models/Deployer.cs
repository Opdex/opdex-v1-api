using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models
{
    public class Deployer
    {
        public Deployer(string address, string owner, ulong createdBlock, ulong modifiedBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (createdBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(createdBlock));
            }
            
            if (modifiedBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(modifiedBlock));
            }
            
            Address = address;
            Owner = owner;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public Deployer(long id, string address, string owner, ulong createdBlock, ulong modifiedBlock)
        {
            Id = id;
            Address = address;
            Owner = owner;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public long Id { get; }
        public string Address { get; }
        public string Owner { get; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; }
    }
}