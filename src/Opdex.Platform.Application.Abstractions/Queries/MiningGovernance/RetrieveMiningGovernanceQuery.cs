using Opdex.Platform.Common.Queries;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernance
{
    public class RetrieveMiningGovernanceQuery : FindQuery<Domain.Models.MiningGovernance>
    {
        public RetrieveMiningGovernanceQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}