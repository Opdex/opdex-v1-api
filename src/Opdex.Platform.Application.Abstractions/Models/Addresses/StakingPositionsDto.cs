using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Addresses;

public class StakingPositionsDto
{
    public IEnumerable<StakingPositionDto> Positions { get; set; }
    public CursorDto Cursor { get; set; }
}