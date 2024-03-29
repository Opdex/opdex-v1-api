using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults;

public class VaultCertificateDto
{
    public Address Owner { get; set; }
    public FixedDecimal Amount { get; set; }
    public ulong VestingStartBlock { get; set; }
    public ulong VestingEndBlock { get; set; }
    public bool Redeemed { get; set; }
    public bool Revoked { get; set; }
    public ulong CreatedBlock { get; set; }
    public ulong ModifiedBlock { get; set; }
    public IEnumerable<ulong> Proposals { get; set; }
}
