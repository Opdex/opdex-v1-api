using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveCirrusVaultTotalSupplyQuery : IRequest<string>
    {
        public RetrieveCirrusVaultTotalSupplyQuery(string vaultAddress, ulong blockHeight)
        {
            VaultAddress = vaultAddress.HasValue() ? vaultAddress : throw new ArgumentNullException(nameof(vaultAddress), "Vault address must be set.");
            BlockHeight = blockHeight;
        }

        public string VaultAddress { get; }
        public ulong BlockHeight { get; }
    }
}
