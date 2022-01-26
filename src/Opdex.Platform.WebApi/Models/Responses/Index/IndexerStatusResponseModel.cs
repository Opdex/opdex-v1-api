using Opdex.Platform.Domain.Models;
using Opdex.Platform.WebApi.Models.Responses.Blocks;
using System;

namespace Opdex.Platform.WebApi.Models.Responses.Index;

/// <summary>
/// Current status of the indexer
/// </summary>
public class IndexerStatusResponseModel
{
    /// <summary>
    /// Last indexed block details
    /// </summary>
    public BlockResponseModel LatestBlock { get; set; }

    /// <summary>
    /// True if the indexing process is available, otherwise false
    /// </summary>
    /// <example>true</example>
    public bool Available { get; set; }

    /// <summary>
    /// True if the indexer is currently executing, otherwise false
    /// </summary>
    /// <example>true</example>
    public bool Locked { get; set; }

    /// <summary>
    /// Unique identifier for the application that is currently indexing
    /// </summary>
    /// <example>bc399269-74cb-4147-a7c3-f2d296fa4a9b</example>
    public string InstanceId { get; set; }

    /// <summary>
    /// Reason that the indexing process is currently executing
    /// </summary>
    /// <example>Rewind</example>
    public IndexLockReason? Reason { get; set; }

    /// <summary>
    /// Timestamp of the current status change
    /// </summary>
    /// <example>2022-01-01T00:00:00Z</example>
    public DateTime ModifiedDate { get; set; }
}
