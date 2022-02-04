using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.Tokens;

public class InterfluxTokenContractSummary
{
    public Address Owner { get; private set; }

    public ExternalChainType NativeChain { get; private set; }

    public string NativeAddress { get; private set; }

    public void SetInterfluxDetails(Address owner, ExternalChainType nativeChain, string nativeAddress)
    {
        if (owner == Address.Empty) throw new ArgumentNullException(nameof(owner));
        if (!nativeChain.IsValid()) throw new ArgumentOutOfRangeException(nameof(nativeChain), "Unrecognised native chain");
        Owner = owner;
        NativeChain = nativeChain;
        NativeAddress = nativeAddress;
    }
}
