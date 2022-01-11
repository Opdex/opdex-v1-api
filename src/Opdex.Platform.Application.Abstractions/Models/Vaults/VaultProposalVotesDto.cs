using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults;

public class VaultProposalVotesDto
{
    public IEnumerable<VaultProposalVoteDto> Votes { get; set; }
    public CursorDto Cursor { get; set; }
}
