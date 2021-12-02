using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultGovernanceCommand : IRequest<ulong>
{
    public MakeVaultGovernanceCommand(VaultGovernance vault, ulong blockHeight, bool refreshSupply = false)
    {
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height cannot be zero.");
        Vault = vault ?? throw new ArgumentNullException(nameof(vault));
        BlockHeight = blockHeight;
        RefreshSupply = refreshSupply;
    }

    public VaultGovernance Vault { get; }
    public ulong BlockHeight { get; }
    public bool RefreshSupply { get; }
    public bool Refresh => RefreshSupply;
}
