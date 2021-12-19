using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

public class RawTransactionDto
{
    public VOutDto[] Vout { get; set; }
}

public class VOutDto
{
    public ScriptPubKeyDto ScriptPubKey { get; set; }
}

public class ScriptPubKeyDto
{
    public Address[] Addresses { get; set; }
}
