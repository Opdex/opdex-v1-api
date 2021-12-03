using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet;

/// <summary>
/// Details for an allowance that has been approved, where tokens can be spent by another address on behalf of the owner.
/// </summary>
public class ApprovedAllowanceResponseModel
{
    /// <summary>
    /// Amount of SRC tokens.
    /// </summary>
    /// <example>"500.00000000"</example>
    public FixedDecimal Allowance { get; set; }

    /// <summary>
    /// Address of the SRC token owner.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Owner { get; set; }

    /// <summary>
    /// Address approved to spend the tokens.
    /// </summary>
    /// <example>t8XpH1pNYDgCnqk91ZQKLgpUVeJ7XmomLT</example>
    public Address Spender { get; set; }

    /// <summary>
    /// Address of the SRC token.
    /// </summary>
    /// <example>tGSk2dVENuqAQ2rNXbui37XHuurFCTqadD</example>
    public Address Token { get; set; }
}