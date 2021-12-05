using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.VaultGovernances;

public class ProcessCreateVaultProposalLogCommandHandler : IRequestHandler<ProcessCreateVaultProposalLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessCreateVaultProposalLogCommandHandler> _logger;

    public ProcessCreateVaultProposalLogCommandHandler(IMediator mediator, ILogger<ProcessCreateVaultProposalLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessCreateVaultProposalLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (vault == null) return false;

            var proposal = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.Log.ProposalId, findOrThrow: false));
            proposal ??= new VaultProposal(request.Log.ProposalId, vault.Id, request.Sender, request.Log.Wallet, request.Log.Amount, request.Log.Description,
                                           request.Log.Type, request.Log.Status, request.Log.Expiration, request.BlockHeight);

            if (request.BlockHeight < proposal.ModifiedBlock)
            {
                return true;
            }

            var proposalUpdated =  await _mediator.Send(new MakeVaultProposalCommand(proposal, request.BlockHeight)) > 0;

            if (proposal.Type == VaultProposalType.Create)
            {
                var vaultUpdated = await _mediator.Send(new MakeVaultGovernanceCommand(vault, request.BlockHeight,
                                                                                       refreshProposedSupply: true)) > 0;

                if (!vaultUpdated) _logger.LogError("Failure updating the vault governance for proposed amount.");
            }

            return proposalUpdated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(CreateVaultProposalLog)}");

            return false;
        }
    }
}
