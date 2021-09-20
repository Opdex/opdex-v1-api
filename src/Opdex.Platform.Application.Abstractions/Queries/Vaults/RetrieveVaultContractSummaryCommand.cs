using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveVaultContractSummaryCommand : IRequest<VaultContractSummary>
    {
        public RetrieveVaultContractSummaryCommand(Address vault, ulong blockHeight, bool includeGenesis = false, bool includeLockedToken = false,
                                                   bool includeOwner = false, bool includeSupply = false)
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
            IncludeGenesis = includeGenesis;
            IncludeLockedToken = includeLockedToken;
            IncludeOwner = includeOwner;
            IncludeSupply = includeSupply;
        }

        public Address Vault { get; }
        public ulong BlockHeight { get; }
        public bool IncludeGenesis { get; }
        public bool IncludeLockedToken { get; }
        public bool IncludeOwner { get; }
        public bool IncludeSupply { get; }
    }
}
