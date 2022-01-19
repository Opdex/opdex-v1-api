using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;
using Opdex.Platform.WebApi.Models.Responses.MiningPools;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools;

/// <summary>
/// Liquidity pool details.
/// </summary>
public class LiquidityPoolResponseModel
{
    /// <summary>
    /// Address of the liquidity pool.
    /// </summary>
    /// <example>tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L</example>
    public Address Address { get; set; }

    /// <summary>
    /// Name of the liquidity pool.
    /// </summary>
    /// <example>TBTC-TCRS</example>
    public string Name { get; set; }

    /// <summary>
    /// Transaction fee percentage for swaps.
    /// </summary>
    /// <example>0.3</example>
    public decimal TransactionFeePercent { get; set; }

    /// <summary>
    /// Tokens involved in the pool.
    /// </summary>
    public LiquidityPoolTokenGroupResponseModel Tokens { get; set; }

    /// <summary>
    /// The governance mining pool associated with the liquidity pool in a staking market.
    /// </summary>
    public MiningPoolResponseModel MiningPool { get; set; }

    /// <summary>
    /// Summary for the pool.
    /// </summary>
    public LiquidityPoolSummaryResponseModel Summary { get; set; }
}
