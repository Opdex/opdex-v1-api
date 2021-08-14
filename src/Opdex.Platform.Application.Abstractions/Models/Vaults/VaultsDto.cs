using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults
{
    public class VaultsDto
    {
        public IEnumerable<VaultDto> Vaults { get; set; }
        public CursorDto Cursor { get; set; }
    }
}
