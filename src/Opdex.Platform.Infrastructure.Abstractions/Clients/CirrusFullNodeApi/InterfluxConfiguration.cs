using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;

public class InterfluxConfiguration : IValidatable
{
    public Address MultiSigContractAddress { get; set; }

    // public Address MultiSigContract => new Address(MultiSigContractAddress);

    public void Validate()
    {
        if (MultiSigContractAddress == Address.Empty || MultiSigContractAddress == Address.Cirrus)
        {
            throw new ArgumentException(nameof(MultiSigContractAddress), "MultiSig contract address must be network address");
        }
    }
}
