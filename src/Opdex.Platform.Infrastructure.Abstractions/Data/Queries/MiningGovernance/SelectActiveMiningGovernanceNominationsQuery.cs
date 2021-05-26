using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernance
{
    public class SelectActiveMiningGovernanceNominationsQuery : IRequest<IEnumerable<MiningGovernanceNomination>>
    {
        
    }
}