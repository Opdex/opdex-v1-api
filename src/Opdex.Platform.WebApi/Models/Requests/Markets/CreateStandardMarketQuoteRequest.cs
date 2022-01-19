using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Markets;

/// <summary>
/// Request to quote the creation of a standard market.
/// </summary>
public class CreateStandardMarketQuoteRequest
{
    [Required]
    public Address MarketOwner { get; set; }

    [Required]
    public uint TransactionFee { get; set; }

    [Required]
    public bool AuthPoolCreators { get; set; }

    [Required]
    public bool AuthLiquidityProviders { get; set; }

    [Required]
    public bool AuthTraders { get; set; }

    [Required]
    public bool EnableMarketFee { get; set; }
}