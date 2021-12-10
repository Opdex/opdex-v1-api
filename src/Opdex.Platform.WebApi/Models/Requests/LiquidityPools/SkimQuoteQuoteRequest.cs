using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

/// <summary>
/// Request to quote skimming excess token balances from a pool.
/// </summary>
public class SkimQuoteRequest
{
    /// <summary>
    /// Address of the recipient for the skimmed tokens.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    [Required]
    public Address Recipient { get; set; }
}