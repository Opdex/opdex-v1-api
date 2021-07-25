using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults
{
    public class CertificatesDto
    {
        public IEnumerable<CertificateDto> Certificates { get; set; }
        public CursorDto Cursor { get; set; }
    }
}
