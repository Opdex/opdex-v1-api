using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens
{
    public class TokenSnapshotsDto
    {
        public Address Token { get; set; }

        public IEnumerable<TokenSnapshotDto> Snapshots { get; set; }

        public CursorDto Cursor { get; set; }
    }
}
