using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

public class MiningGovernanceNominationCirrusDto
{
    public Address StakingPool { get; set; }
    public UInt256 Weight { get; set; }
}