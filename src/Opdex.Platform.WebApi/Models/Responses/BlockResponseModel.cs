using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses;

/// <summary>
/// Details for a block.
/// </summary>
public class BlockResponseModel
{
    /// <summary>
    /// SHA-256 block hash.
    /// </summary>
    /// <example>095b8770671557437ecd2e45b634fca27eced555985e68ff72c5fd3818c07034</example>
    public string Hash { get; set; }

    /// <summary>
    /// Block number.
    /// </summary>
    /// <example>500000</example>
    public ulong Height { get; set; }

    /// <summary>
    /// Block timestamp as determined by the block producer.
    /// </summary>
    /// <example>2022-01-01T00:00:30Z</example>
    public DateTime Time { get; set; }

    /// <summary>
    /// Network-adjusted timestamp.
    /// </summary>
    /// <example>2022-01-01T00:00:00Z</example>
    public DateTime MedianTime { get; set; }
}
