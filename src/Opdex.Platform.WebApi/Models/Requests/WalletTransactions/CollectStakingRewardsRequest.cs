using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions;

public class CollectStakingRewardsRequest
{
    /// <summary>
    /// An option to liquidate earned liquidity pool tokens from staking back into
    /// the pool's reserves tokens.
    /// </summary>
    [Required]
    public bool Liquidate { get; set; }
}