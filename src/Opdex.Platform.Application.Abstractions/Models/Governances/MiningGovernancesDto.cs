using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Governances
{
    public class MiningGovernancesDto
    {
        public IEnumerable<MiningGovernanceDto> Governances { get; set; }
        public CursorDto Cursor { get; set; }
    }
}
