using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultProposalPledgeCommand : IRequest<ulong>
{
    public MakeVaultProposalPledgeCommand(VaultProposalPledge pledge, ulong blockHeight, bool refreshPledge = false)
    {
        Pledge = pledge ?? throw new ArgumentNullException(nameof(pledge), "Vault pledge must be provided.");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        RefreshPledge = refreshPledge;
    }

    public VaultProposalPledge Pledge { get; }
    public ulong BlockHeight { get; }
    public bool RefreshPledge { get; }
}
