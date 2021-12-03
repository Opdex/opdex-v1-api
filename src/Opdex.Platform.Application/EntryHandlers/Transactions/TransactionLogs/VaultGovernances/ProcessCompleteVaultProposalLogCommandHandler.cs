using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.VaultGovernances;

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
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (vault == null) return false;

            var proposal = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.Log.ProposalId, findOrThrow: false));
            if (proposal == null) return false;

            if (request.BlockHeight < proposal.ModifiedBlock) return true;

            proposal.Update(request.Log, request.BlockHeight);

            return await _mediator.Send(new MakeVaultProposalCommand(proposal, request.BlockHeight)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(CompleteVaultProposalLog)}");

            return false;
        }
    }
}
