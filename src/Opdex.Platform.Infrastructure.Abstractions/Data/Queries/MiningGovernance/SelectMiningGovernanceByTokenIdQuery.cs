using System;
using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernance
{
    public class SelectMiningGovernanceByTokenIdQuery : IRequest<Domain.MiningGovernance>
    {
        public SelectMiningGovernanceByTokenIdQuery(long tokenId)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            TokenId = tokenId;
        }
        
        public long TokenId { get; }
    }
}