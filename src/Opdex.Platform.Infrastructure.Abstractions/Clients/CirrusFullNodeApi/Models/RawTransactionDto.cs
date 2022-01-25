using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

public class RawTransactionDto
{
    public RawTransactionDto()
    {
        Vout = Array.Empty<VOutDto>();
    }

    public VOutDto[] Vout { get; set; }
}

public class VOutDto
{
    public ScriptPubKeyDto ScriptPubKey { get; set; }
}

public class ScriptPubKeyDto
{
    public ScriptPubKeyDto()
    {
        Addresses = Array.Empty<Address>();
    }

    public Address[] Addresses { get; set; }
}
