using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveVaultContractSummaryCommand : IRequest<VaultContractSummary>
    {
        public RetrieveVaultContractSummaryCommand(Address vault, ulong blockHeight)
        {
            if (vault == Address.Empty)
            {
                throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Vault = vault;
            BlockHeight = blockHeight;
        }

        public Address Vault { get; }
        public ulong BlockHeight { get; }
    }
}
