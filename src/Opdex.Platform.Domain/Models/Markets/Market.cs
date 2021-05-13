using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class Market
    {
        public Market(string address, long deployerId, long? stakingTokenId, string owner, bool authPoolCreators, bool authProviders, bool authTraders, uint fee)
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

            Address = address;
            DeployerId = deployerId;
            StakingTokenId = stakingTokenId;
            Owner = owner;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            Fee = fee;
        }
        
        public Market(long id, string address, long deployerId, long? stakingTokenId, string owner, bool authPoolCreators, bool authProviders, bool authTraders, uint fee)
        {
            Id = id;
            Address = address;
            DeployerId = deployerId;
            StakingTokenId = stakingTokenId;
            Owner = Owner;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            Fee = fee;
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
    }
}