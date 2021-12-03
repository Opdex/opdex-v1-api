using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Markets;

public class MarketSnapshotsDto
{
    public IEnumerable<MarketSnapshotDto> Snapshots { get; set; }
    public CursorDto Cursor { get; set; }
}