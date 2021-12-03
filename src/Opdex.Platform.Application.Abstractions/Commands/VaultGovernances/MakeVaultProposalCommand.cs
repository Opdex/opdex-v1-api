using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultProposalCommand : IRequest<ulong>
{
    public MakeVaultProposalCommand(VaultProposal proposal, ulong blockHeight, bool refreshProposal = false)
    {
        Proposal = proposal ?? throw new ArgumentNullException(nameof(proposal), "Vault proposal must be provided.");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentNullException(nameof(blockHeight), "Block height must be greater than zero.");
        RefreshProposal = refreshProposal;
    }

    public VaultProposal Proposal { get; }
    public ulong BlockHeight { get; }
    public bool RefreshProposal { get; }
}
