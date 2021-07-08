using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances
{
    public class RetrieveMiningGovernanceQuery : FindQuery<MiningGovernance>
    {
        public RetrieveMiningGovernanceQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}
