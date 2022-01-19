using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

/// <summary>
/// Volume summary.
/// </summary>
public class VolumeResponseModel
{
    /// <summary>
    /// USD value of the daily volume.
    /// </summary>
    /// <example>50000000.50</example>
    public decimal DailyUsd { get; set; }
}