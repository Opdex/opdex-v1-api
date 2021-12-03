using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningGovernances;

/// <summary>
/// Reward mining pool event.
/// </summary>
public class RewardMiningPoolEvent : TransactionEvent
{
    /// <summary>
    /// Address of the staking pool.
    /// </summary>
    /// <example>tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L</example>
    public Address StakingPool { get; set; }

    /// <summary>
    /// Address of the mining pool.
    /// </summary>
    /// <example>tNgQhNxvachxKGvRonk2S8nrpYi44carYv</example>
    public Address MiningPool { get; set; }

    /// <summary>
    /// Governance token reward amount.
    /// </summary>
    /// <example>"50000.00000000"</example>
    public FixedDecimal Amount { get; set; }
}