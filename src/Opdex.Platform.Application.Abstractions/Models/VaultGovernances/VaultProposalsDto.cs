using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.VaultGovernances;

public class VaultProposalsDto
{
    public IEnumerable<VaultProposalDto> Proposals { get; set; }
    public CursorDto Cursor { get; set; }
}
