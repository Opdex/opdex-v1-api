using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances
{
    public class SelectActiveMiningGovernanceNominationsQuery : IRequest<IEnumerable<MiningGovernanceNomination>>
    {
    }
}
