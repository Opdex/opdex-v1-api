using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class LiquidityPoolSnapshotsDto
    {
        public IEnumerable<LiquidityPoolSnapshotDto> Snapshots { get; set; }

        public CursorDto Cursor { get; set; }
    }
}
