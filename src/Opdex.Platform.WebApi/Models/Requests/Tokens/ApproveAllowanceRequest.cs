using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Tokens;

/// <summary>
/// Request to approve an allowance.
/// </summary>
public class ApproveAllowanceRequest
{
    /// <summary>
    /// Amount of SRC tokens to approve.
    /// </summary>
    /// <example>"500.000000000000000000"</example>
    [Required]
    public FixedDecimal Amount { get; set; }

    /// <summary>
    /// Address of the allowance spender.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    [Required]
    public Address Spender { get; set; }
}