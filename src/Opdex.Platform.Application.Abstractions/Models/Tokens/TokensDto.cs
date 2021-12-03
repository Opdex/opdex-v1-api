using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens;

public class TokensDto
{
    public IEnumerable<TokenDto> Tokens { get; set; }
    public CursorDto Cursor { get; set; }
}