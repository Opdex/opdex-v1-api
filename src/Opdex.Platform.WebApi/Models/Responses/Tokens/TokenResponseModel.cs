using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens;

/// <summary>
/// Token details.
/// </summary>
public class TokenResponseModel
{
    /// <summary>
    /// Address of the token.
    /// </summary>
    /// <example>tFPedNjm3q8N9HD7wSVTNK5Kvw96332P1o</example>
    public Address Address { get; set; }

    /// <summary>
    /// Name of the token.
    /// </summary>
    /// <example>Opdex Fan Token</example>
    public string Name { get; set; }

    /// <summary>
    /// Ticker symbol for the token.
    /// </summary>
    /// <example>OFT</example>
    public string Symbol { get; set; }

    /// <summary>
    /// The total number of decimal places the token has.
    /// </summary>
    /// <example>8</example>
    [Range(0, double.MaxValue)]
    public int Decimals { get; set; }

    /// <summary>
    /// The total number of satoshis per full token.
    /// </summary>
    /// <example>100000000</example>
    [Range(0, double.MaxValue)]
    public ulong Sats { get; set; }

    /// <summary>
    /// The total supply of the token as stored in contract.
    /// </summary>
    /// <example>"2100000000000000"</example>
    public FixedDecimal TotalSupply { get; set; }

    /// <summary>
    /// A summary including the USD price of the token and daily price change percentage if exists. Market tokens receive
    /// pricing specific to that market.
    /// </summary>
    public TokenSummaryResponseModel Summary { get; set; }
}