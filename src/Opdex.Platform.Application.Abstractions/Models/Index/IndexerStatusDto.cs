using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.Domain.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.Models.Index;

public class IndexerStatusDto
{
    public BlockDto LatestBlock { get; set; }

    public bool Available { get; set; }

    public bool Locked { get; set; }

    public string InstanceId { get; set; }

    public IndexLockReason Reason { get; set; }

    public DateTime ModifiedDate { get; set; }
}
