using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations
{
    public class RetrieveActiveMiningGovernanceNominationsQuery : IRequest<IEnumerable<MiningGovernanceNomination>>
    {

    }
}
