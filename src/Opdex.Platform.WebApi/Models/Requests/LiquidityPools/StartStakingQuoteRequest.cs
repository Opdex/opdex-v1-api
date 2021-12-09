using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

/// <summary>
/// Request to quote starting staking in a pool.
/// </summary>
public class StartStakingQuoteRequest
{
    /// <summary>
    /// Amount of governance tokens to stake.
    /// </summary>
    /// <example>"1000.00000000"</example>
    [Required]
    public FixedDecimal Amount { get; set; }
}