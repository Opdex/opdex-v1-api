using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

/// <summary>
/// Request to quote removing liquidity from a pool.
/// </summary>
public class RemoveLiquidityQuoteRequest
{
    /// <summary>
    /// Amount of liquidity pool tokens to remove.
    /// </summary>
    /// <example>"250.00000000"</example>
    [Required]
    public FixedDecimal Liquidity { get; set; }

    /// <summary>
    /// Minimum amount of CRS tokens to receive, or fail the transaction.
    /// </summary>
    /// <example>"1500.00000000"</example>
    [Required]
    public FixedDecimal AmountCrsMin { get; set; }

    /// <summary>
    /// Minimum amount of SRC tokens to receive, or fail the transaction.
    /// </summary>
    /// <example>"3.000000000000000000"</example>
    [Required]
    public FixedDecimal AmountSrcMin { get; set; }

    /// <summary>
    /// Address of the recipient of the removed liquidity.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    [Required]
    public Address Recipient { get; set; }

    /// <summary>
    /// Block number limit that the transaction is valid through.
    /// </summary>
    /// <remarks>A 0 deadline is equivalent to no deadline. Anything else must be greater than the current chain height.</remarks>
    /// <example>500000</example>
    public ulong Deadline { get; set; }
}