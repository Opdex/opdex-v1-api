using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Addresses
{
    public class MiningPositionsDto
    {
        public IEnumerable<MiningPositionDto> Positions { get; set; }
        public CursorDto Cursor { get; set; }
    }
}
