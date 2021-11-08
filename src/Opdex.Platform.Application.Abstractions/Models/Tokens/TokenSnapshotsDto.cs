using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens
{
    public class TokenSnapshotsDto
    {
        public IEnumerable<TokenSnapshotDto> Snapshots { get; set; }

        public CursorDto Cursor { get; set; }
    }
}
