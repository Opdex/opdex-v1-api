using Opdex.Platform.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

/// <summary>
/// Request to quote adding liquidity to a pool.
/// </summary>
public class AddLiquidityQuoteRequest
{
    /// <summary>
    /// Amount of CRS tokens to provide.
    /// </summary>
    /// <example>"500.00000000"</example>
    [Required]
    public FixedDecimal AmountCrs { get; set; }

    /// <summary>
    /// Amount of SRC tokens to provide.
    /// </summary>
    /// <example>"2500.00000000000000000"</example>
    [Required]
    public FixedDecimal AmountSrc { get; set; }

    /// <summary>
    /// Minimum amount of SRC tokens acceptable to provide.
    /// </summary>
    /// <example>"2490.00000000000000000"</example>
    [Required]
    public FixedDecimal AmountSrcMin { get; set; }

    /// <summary>
    /// Minimum amount of CRS tokens acceptable to provide.
    /// </summary>
    /// <example>"495.00000000"</example>
    [Required]
    public FixedDecimal AmountCrsMin { get; set; }

    /// <summary>
    /// Recipient of the liquidity pool tokens.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    [Required]
    public Address Recipient { get; set; }

    /// <summary>
    /// Block number limit that the transaction is valid through.
    /// </summary>
    /// <remarks>A 0 deadline is equivalent to no deadline. Anything else must be greater than the current chain height.</remarks>
    /// <example>0</example>
    [Range(0, double.MaxValue)]
    public ulong Deadline { get; set; }
}