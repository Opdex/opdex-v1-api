using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults;

public class VaultProposalPledgesDto
{
    public IEnumerable<VaultProposalPledgeDto> Pledges { get; set; }
    public CursorDto Cursor { get; set; }
}
