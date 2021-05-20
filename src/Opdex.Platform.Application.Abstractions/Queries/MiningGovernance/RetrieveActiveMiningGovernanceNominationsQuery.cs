using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernance
{
    public class RetrieveActiveMiningGovernanceNominationsQuery : IRequest<IEnumerable<MiningGovernanceNomination>>
    {
        
    }
}