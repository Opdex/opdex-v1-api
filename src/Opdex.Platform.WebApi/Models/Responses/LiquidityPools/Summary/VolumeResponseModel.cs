using NJsonSchema.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

public class VolumeResponseModel
{
    /// <summary>
    /// The daily volume amount in USD.
    /// </summary>
    [NotNull]
    [Range(double.MinValue, double.MaxValue)]
    public decimal DailyUsd { get; set; }
}