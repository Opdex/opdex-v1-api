using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.VaultGovernances;

public class VaultGovernanceDto
{
    public Address Vault { get; set; }
    public Address Token { get; set; }
    public FixedDecimal TokensUnassigned { get; set; }
    public FixedDecimal TokensProposed { get; set; }
    public FixedDecimal TokensLocked { get; set; }
    public FixedDecimal TotalPledgeMinimum { get; set; }
    public FixedDecimal TotalVoteMinimum { get; set; }
    public ulong VestingDuration { get; set; }
}
