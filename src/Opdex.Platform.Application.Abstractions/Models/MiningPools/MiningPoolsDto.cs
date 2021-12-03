using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.MiningPools;

public class MiningPoolsDto
{
    public IEnumerable<MiningPoolDto> MiningPools { get; set; }
    public CursorDto Cursor { get; set; }
}