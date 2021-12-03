using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools;

public class LiquidityPoolsDto
{
    public IEnumerable<LiquidityPoolDto> LiquidityPools { get; set; }
    public CursorDto Cursor { get; set; }
}