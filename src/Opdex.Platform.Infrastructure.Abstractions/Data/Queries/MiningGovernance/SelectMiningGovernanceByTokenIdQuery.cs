using System;
using MediatR;
using Opdex.Platform.Common.Queries;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernance
{
    public class SelectMiningGovernanceByTokenIdQuery : FindQuery<Domain.MiningGovernance>
    {
        public SelectMiningGovernanceByTokenIdQuery(long tokenId, bool findOrThrow = true) : base(findOrThrow)
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