using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults;

public class ProcessCompleteVaultProposalLogCommandHandler : IRequestHandler<ProcessCompleteVaultProposalLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessCompleteVaultProposalLogCommandHandler> _logger;

    public ProcessCompleteVaultProposalLogCommandHandler(IMediator mediator, ILogger<ProcessCompleteVaultProposalLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessCompleteVaultProposalLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (vault == null) return false;

            var proposal = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.Log.ProposalId, findOrThrow: false));
            if (proposal == null) return false;

            if (request.BlockHeight < proposal.ModifiedBlock) return true;

            proposal.Update(request.Log, request.BlockHeight);

            var proposalUpdated = await _mediator.Send(new MakeVaultProposalCommand(proposal, request.BlockHeight)) > 0;

            var isMinimumProposal = proposal.Type == VaultProposalType.TotalPledgeMinimum || proposal.Type == VaultProposalType.TotalVoteMinimum;
            var isCreate = proposal.Type == VaultProposalType.Create;
            var isRevoke = proposal.Type == VaultProposalType.Revoke;
            var refreshProposedSupply = false;
            var refreshUnassignedSupply = false;

            if (request.Log.Approved)
            {
                if (isMinimumProposal) vault.Update(request.Log, proposal, request.BlockHeight);
                else if (isCreate)
                {
                    refreshProposedSupply = true;
                    refreshUnassignedSupply = true;
                }
                else if (isRevoke) refreshUnassignedSupply = true;
            }
            else if (isCreate) refreshProposedSupply = true;

            var vaultUpdated = await _mediator.Send(new MakeVaultCommand(vault, request.BlockHeight,
                                                                         refreshProposedSupply: refreshProposedSupply,
                                                                         refreshUnassignedSupply: refreshUnassignedSupply)) > 0;

            if (!vaultUpdated) _logger.LogError("Failure updating the vault with proposal changes.");

            return proposalUpdated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(CompleteVaultProposalLog)}");

            return false;
        }
    }
}
