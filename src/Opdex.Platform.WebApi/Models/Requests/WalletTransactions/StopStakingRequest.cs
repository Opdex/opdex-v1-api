using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions;

public class StopStakingRequest
{
    /// <summary>
    /// An option to liquidate earned liquidity pool tokens from staking back into
    /// the pool's reserves tokens.
    /// </summary>
    [Required]
    public bool Liquidate { get; set; }

    /// <summary>
    /// The amount of tokens to stop staking.
    /// </summary>
    [Required]
    public FixedDecimal Amount { get; set; }
}