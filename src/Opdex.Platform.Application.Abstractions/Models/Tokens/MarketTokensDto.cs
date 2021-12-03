using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens;

public class MarketTokensDto
{
    public IEnumerable<MarketTokenDto> Tokens { get; set; }
    public CursorDto Cursor { get; set; }
}