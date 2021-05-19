using System;
using MediatR;
using Opdex.Platform.Common.Queries;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernance
{
    public class RetrieveMiningGovernanceByTokenIdQuery : FindQuery<Domain.MiningGovernance>
    {
        public RetrieveMiningGovernanceByTokenIdQuery(long tokenId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (tokenId < 1)
            {
                throw new ArgumentNullException(nameof(tokenId));
            }

            TokenId = tokenId;
        }
        
        public long TokenId { get; }
    }
}