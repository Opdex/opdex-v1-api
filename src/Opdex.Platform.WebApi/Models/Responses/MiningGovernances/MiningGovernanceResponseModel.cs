using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.MiningGovernances;

/// <summary>
/// Mining governance details.
/// </summary>
public class MiningGovernanceResponseModel
{
    /// <summary>
    /// Address of the mining governance contract.
    /// </summary>
    /// <example>tVfGTqrToiTU9bfnvD5UDC5ZQVY4oj4jrc</example>
    public Address Address { get; set; }

    /// <summary>
    /// Block number at the end of the current nomination period.
    /// </summary>
    /// <example>500000</example>
    [Range(1, double.MaxValue)]
    public ulong PeriodEndBlock { get; set; }

    /// <summary>
    /// Remaining blocks for the current nomination period.
    /// </summary>
    /// <example>10000</example>
    [Range(0, double.MaxValue)]
    public ulong PeriodRemainingBlocks { get; set; }

    /// <summary>
    /// The number of blocks for each nomination period.
    /// </summary>
    /// <example>10000</example>
    [Range(1, double.MaxValue)]
    public ulong PeriodBlockDuration { get; set; }

    /// <summary>
    /// Number of nomination periods before mining governance rewards are reset. Rewards reset upon distribution to 48 nominations.
    /// </summary>
    /// <example>3</example>
    [Range(0, double.MaxValue)]
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
}