using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class MakeMarketCommand : IRequest<long>
    {
        public MakeMarketCommand(string address, long deployerId, long? stakingTokenId, bool authPoolCreators, bool authProviders, bool authTraders, uint fee)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (fee > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(fee));
            }

            if (deployerId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(deployerId));
            }
            
            Address = address;
            DeployerId = deployerId;
            StakingTokenId = stakingTokenId;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            Fee = fee;
        }
        
        public string Address { get; }
        public long DeployerId { get; }
        public long? StakingTokenId { get; }
        public bool AuthPoolCreators { get; }
        public bool AuthProviders { get; }
        public bool AuthTraders { get; }
        public uint Fee { get; }
    }
}