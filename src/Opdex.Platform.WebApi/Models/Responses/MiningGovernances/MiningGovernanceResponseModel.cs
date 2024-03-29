using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.MiningGovernances;

/// <summary>
/// Mining governance details.
/// </summary>
public class MiningGovernanceResponseModel
{
    /// <summary>
    /// Address of the mining governance contract.
    /// </summary>
    /// <example>tKFkNiL5KJ3Q4br929i6hHbB4X4mt1MigF</example>
    public Address Address { get; set; }

    /// <summary>
    /// Block number at the end of the current nomination period.
    /// </summary>
    /// <example>500000</example>
    public ulong PeriodEndBlock { get; set; }

    /// <summary>
    /// Remaining blocks for the current nomination period.
    /// </summary>
    /// <example>10000</example>
    public ulong PeriodRemainingBlocks { get; set; }

    /// <summary>
    /// The number of blocks for each nomination period.
    /// </summary>
    /// <example>10000</example>
    public ulong PeriodBlockDuration { get; set; }

    /// <summary>
    /// Number of nomination periods before mining governance rewards are reset. Rewards reset upon distribution to 48 nominations.
    /// </summary>
    /// <example>3</example>
    public uint PeriodsUntilRewardReset { get; set; }

    /// <summary>
    /// Current governance token reward amount per nomination.
    /// </summary>
    /// <example>625000</example>
    public FixedDecimal MiningPoolRewardPerPeriod { get; set; }

    /// <summary>
    /// Current governance token reward amount for each nomination period.
    /// </summary>
    /// <example>2500000</example>
    public FixedDecimal TotalRewardsPerPeriod { get; set; }

    /// <summary>
    /// Address of the governance token.
    /// </summary>
    /// <example>tTTuKbCR2UnsEByXBp1ynBz91J2yz63h1c</example>
    public Address MinedToken { get; set; }

    /// <summary>
    /// Block number at which the entity was created.
    /// </summary>
    /// <example>2500000</example>
    public ulong CreatedBlock { get; set; }

    /// <summary>
    /// Block number at which the entity state was last modified.
    /// </summary>
    /// <example>3000000</example>
    public ulong ModifiedBlock { get; set; }
}
