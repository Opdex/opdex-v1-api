using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances
{
    public class SelectMiningGovernanceQuery : FindQuery<MiningGovernance>
    {
        public SelectMiningGovernanceQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}
