using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.MiningGovernances;

public class MiningGovernancesDto
{
    public IEnumerable<MiningGovernanceDto> MiningGovernances { get; set; }
    public CursorDto Cursor { get; set; }
}