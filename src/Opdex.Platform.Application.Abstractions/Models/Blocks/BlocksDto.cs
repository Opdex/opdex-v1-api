using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Blocks;

public class BlocksDto
{
    public IEnumerable<BlockDto> Blocks { get; set; }
    public CursorDto Cursor { get; set; }
}
