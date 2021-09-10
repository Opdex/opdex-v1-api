using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveCirrusVaultTotalSupplyQuery : IRequest<UInt256>
    {
        public RetrieveCirrusVaultTotalSupplyQuery(Address vaultAddress, ulong blockHeight)
        {
            VaultAddress = vaultAddress != Address.Empty ? vaultAddress : throw new ArgumentNullException(nameof(vaultAddress), "Vault address must be set.");
            BlockHeight = blockHeight;
        }

        public Address VaultAddress { get; }
        public ulong BlockHeight { get; }
    }
}
