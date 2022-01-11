using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults;

public class VaultProposalsDto
{
    public IEnumerable<VaultProposalDto> Proposals { get; set; }
    public CursorDto Cursor { get; set; }
}
