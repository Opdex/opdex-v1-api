using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults;

public class VaultCertificatesDto
{
    public IEnumerable<VaultCertificateDto> Certificates { get; set; }
    public CursorDto Cursor { get; set; }
}
