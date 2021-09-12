using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults
{
    public class CallCirrusGetVaultTotalSupplyQuery : IRequest<UInt256>
    {
        public CallCirrusGetVaultTotalSupplyQuery(Address vaultAddress, ulong blockHeight)
        {
            VaultAddress = vaultAddress != Address.Empty ? vaultAddress : throw new ArgumentNullException(nameof(vaultAddress), "Vault address must be set.");
            BlockHeight = blockHeight;
        }

        public Address VaultAddress { get; }
        public ulong BlockHeight { get; }
    }
}
