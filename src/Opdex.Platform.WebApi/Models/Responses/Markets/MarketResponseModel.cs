using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.Tokens;

namespace Opdex.Platform.WebApi.Models.Responses.Markets;

/// <summary>
/// Market details.
/// </summary>
public class MarketResponseModel
{
    /// <summary>
    /// Address of the market.
    /// </summary>
    /// <example>tFPedNjm3q8N9HD7wSVTNK5Kvw96332P1o</example>
    [NotNull]
    public Address Address { get; set; }

    /// <summary>
    /// Address of a pending owner.
    /// </summary>
    /// <example>twSVTNK5Kvw96332FPq8N9HD7P1oedNjm3</example>
    public Address PendingOwner { get; set; }

    /// <summary>
    /// Address of the current owner.
    /// </summary>
    /// <example>t7wSVTNK5Kvw96332P1oFPedNjm3q8N9HD</example>
    [NotNull]
    public Address Owner { get; set; }

    /// <summary>
    /// Token details about the markets staking token if exists.
    /// </summary>
    public TokenResponseModel StakingToken { get; set; }

    /// <summary>
    /// Token details of CRS.
    /// </summary>
    [NotNull]
    public TokenResponseModel CrsToken { get; set; }

    /// <summary>
    /// Flag determining if the market has a whitelist of addresses with create liquidity pool permissions.
    /// </summary>
    /// <example>false</example>
    [NotNull]
    public bool AuthPoolCreators { get; set; }

    /// <summary>
    /// Flag determining if the market has a whitelist of addresses with trade permissions.
    /// </summary>
    /// <example>false</example>
    [NotNull]
    public bool AuthTraders { get; set; }

    /// <summary>
    /// Flag determining if the market has a whitelist of addresses with provisioning permissions.
    /// </summary>
    /// <example>false</example>
    [NotNull]
    public bool AuthProviders { get; set; }

    /// <summary>
    /// Flag determining if the market splits transaction fees. Staking markets split fees with stakers, private markets
    /// optionally can split fees with the market owner.
    /// </summary>
    /// <example>true</example>
    [NotNull]
    public bool MarketFeeEnabled { get; set; }

    /// <summary>
    /// The transaction fee of the market for swap transaction.
    /// </summary>
    /// <example>true</example>
    [NotNull]
    public decimal TransactionFee { get; set; }

    /// <summary>
    /// Summary of market statistics include liquidity, volume, rewards etc.
    /// </summary>
    [NotNull]
    public MarketSummaryResponseModel Summary { get; set; }
}
