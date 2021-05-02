using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class MakeMarketCommand : IRequest<long>
    {
        public MakeMarketCommand(string address, bool authPoolCreators, bool authProviders, bool authTraders, uint fee, bool staking)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (fee > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(fee));
            }
            
            Address = address;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            Fee = fee;
            Staking = staking;
        }
        
        public string Address { get; }
        public bool AuthPoolCreators { get; }
        public bool AuthProviders { get; }
        public bool AuthTraders { get; }
        public uint Fee { get; }
        public bool Staking { get; }
    }
}