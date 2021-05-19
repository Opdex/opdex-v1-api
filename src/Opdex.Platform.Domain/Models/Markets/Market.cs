using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class Market
    {
        public Market(string address, long deployerId, long? stakingTokenId, string owner, bool authPoolCreators, bool authProviders, 
            bool authTraders, uint fee, ulong createdBlock, ulong modifiedBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (deployerId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(deployerId));
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
            DeployerId = deployerId;
            StakingTokenId = stakingTokenId;
            Owner = owner;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            Fee = fee;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public Market(long id, string address, long deployerId, long? stakingTokenId, string owner, bool authPoolCreators, bool authProviders,
            bool authTraders, uint fee, ulong createdBlock, ulong modifiedBlock)
        {
            Id = id;
            Address = address;
            DeployerId = deployerId;
            StakingTokenId = stakingTokenId;
            Owner = owner;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            Fee = fee;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public long Id { get; }
        public string Address { get; }
        public long DeployerId { get; }
        public long? StakingTokenId { get; }
        public string Owner { get; }
        public bool AuthPoolCreators { get; }
        public bool AuthProviders { get; }
        public bool AuthTraders { get; }
        public uint Fee { get; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; }
    }
}