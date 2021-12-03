using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

public class StakingResponseModel
{
    /// <summary>
    /// The governance token used for staking.
    /// </summary>
    [NotNull]
    public MarketTokenResponseModel Token { get; set; }

    /// <summary>
    /// The total number of tokens staking.
    /// </summary>
    [NotNull]
    public FixedDecimal Weight { get; set; }

    /// <summary>
    /// The total USD amount staking.
    /// </summary>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal Usd { get; set; }

    /// <summary>
    /// The percentage amount of change in staking weight for the day.
    /// </summary>
    [NotNull]
    [Range(double.MinValue, double.MaxValue)]
    public decimal DailyWeightChangePercent { get; set; }

    /// <summary>
    /// Flag determining if the liquidity pool is nominated for mining.
    /// </summary>
    [NotNull]
    public bool Nominated { get; set; }
}