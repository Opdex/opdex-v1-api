using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults;

/// <summary>
/// Retrieve a vault contract's summary by select retrievable properties.
/// </summary>
public class RetrieveVaultContractSummaryQuery : IRequest<VaultContractSummary>
{
    /// <summary>
    /// Constructor to create the retrieve vault contract summary command.
    /// </summary>
    /// <param name="vault">The address of the vault smart contract.</param>
    /// <param name="blockHeight">The block height to get properties value at.</param>
    /// <param name="includeGenesis">Flag to include the genesis block property, default is false.</param>
    /// <param name="includeLockedToken">Flag to include the locked token property, default is false.</param>
    /// <param name="includePendingOwner">Flag to include the pending owner property, default is false.</param>
    /// <param name="includeOwner">Flag to include the owner property, default is false.</param>
    /// <param name="includeSupply">Flag to include the unassigned supply property, default is false.</param>
    public RetrieveVaultContractSummaryQuery(Address vault, ulong blockHeight, bool includeGenesis = false, bool includeLockedToken = false,
                                             bool includePendingOwner = false, bool includeOwner = false, bool includeSupply = false)
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
        IncludePendingOwner = includePendingOwner;
        IncludeOwner = includeOwner;
        IncludeSupply = includeSupply;
    }

    public Address Vault { get; }
    public ulong BlockHeight { get; }
    public bool IncludeGenesis { get; }
    public bool IncludeLockedToken { get; }
    public bool IncludePendingOwner { get; }
    public bool IncludeOwner { get; }
    public bool IncludeSupply { get; }
}