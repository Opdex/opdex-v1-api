using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults;

public class VaultGovernancesDto
{
    public IEnumerable<VaultGovernanceDto> Vaults { get; set; }
    public CursorDto Cursor { get; set; }
}
