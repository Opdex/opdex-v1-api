using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults
{
    public class CallCirrusGetVaultTotalSupplyQuery : IRequest<string>
    {
        public CallCirrusGetVaultTotalSupplyQuery(string vaultAddress, ulong blockHeight)
        {
            VaultAddress = vaultAddress.HasValue() ? vaultAddress : throw new ArgumentNullException(nameof(vaultAddress), "Vault address must be set.");
            BlockHeight = blockHeight;
        }

        public string VaultAddress { get; }
        public ulong BlockHeight { get; }
    }
}
