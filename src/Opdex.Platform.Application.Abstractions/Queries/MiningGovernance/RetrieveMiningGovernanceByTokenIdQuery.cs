using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernance
{
    public class RetrieveMiningGovernanceByTokenIdQuery : IRequest<Domain.MiningGovernance>
    {
        public RetrieveMiningGovernanceByTokenIdQuery(long tokenId)
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