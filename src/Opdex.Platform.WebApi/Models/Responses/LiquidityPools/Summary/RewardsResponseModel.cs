using NJsonSchema.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

/// <summary>
/// Rewards summary.
/// </summary>
public class RewardsResponseModel
{
    /// <summary>
    /// USD value of rewards to providers, for the current day.
    /// </summary>
    /// <example>50000.50</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal ProviderDailyUsd { get; set; }

    /// <summary>
    /// USD value of rewards to the market, either the owner of a standard market or stakers in a staking market, for the current day.
    /// </summary>
    /// <example>10000.10</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal MarketDailyUsd { get; set; }

    /// <summary>
    /// USD value of all rewards generated from token swaps, for the current day.
    /// </summary>
    /// <example>60000.60</example>
    [NotNull]
    [Range(0, double.MaxValue)]
    public decimal TotalDailyUsd { get; set; }
}