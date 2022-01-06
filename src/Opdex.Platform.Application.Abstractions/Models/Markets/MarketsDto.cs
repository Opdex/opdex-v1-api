using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Markets;

public class MarketsDto
{
    public IEnumerable<MarketDto> Markets { get; set; }
    public CursorDto Cursor { get; set; }
}
