using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.VaultGovernances;

public class VaultCertificateDto
{
    public ulong ProposalId { get; set; }
    public Address Owner { get; set; }
    public FixedDecimal Amount { get; set; }
    public ulong VestingStartBlock { get; set; }
    public ulong VestingEndBlock { get; set; }
    public bool Redeemed { get; set; }
    public bool Revoked { get; set; }
}
