using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models
{
    public class Market
    {
        public Market(string address, bool authPoolCreators, bool authProviders, bool authTraders, uint fee, bool staking)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            Fee = fee;
            Staking = staking;
        }
        
        public Market(long id, string address, bool authPoolCreators, bool authProviders, bool authTraders, uint fee, bool staking)
        {
            Id = id;
            Address = address;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            Fee = fee;
            Staking = staking;
        }
        
        public long Id { get; }
        public string Address { get; }
        public bool AuthPoolCreators { get; }
        public bool AuthProviders { get; }
        public bool AuthTraders { get; }
        public uint Fee { get; }
        public bool Staking { get; }
    }
}