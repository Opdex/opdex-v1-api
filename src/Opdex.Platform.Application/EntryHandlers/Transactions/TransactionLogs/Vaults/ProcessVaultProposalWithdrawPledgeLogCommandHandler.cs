using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Pledges;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults;

public class ProcessVaultProposalWithdrawPledgeLogCommandHandler : IRequestHandler<ProcessVaultProposalWithdrawPledgeLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessVaultProposalWithdrawPledgeLogCommandHandler> _logger;

    public ProcessVaultProposalWithdrawPledgeLogCommandHandler(IMediator mediator, ILogger<ProcessVaultProposalWithdrawPledgeLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessVaultProposalWithdrawPledgeLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (vault == null) return false;

            var proposal = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.Log.ProposalId, findOrThrow: false));
            if (proposal == null) return false;

            proposal.Update(request.Log, request.BlockHeight);

            var pledge = await _mediator.Send(new RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery(vault.Id, proposal.Id,
                                                                                                                   request.Log.Pledger,
                                                                                                                   findOrThrow: false));
            if (pledge == null) return false;

            if (request.BlockHeight < pledge.ModifiedBlock) return true;

            pledge.Update(request.Log, request.BlockHeight);

            var persistedProposal = await _mediator.Send(new MakeVaultProposalCommand(proposal, request.BlockHeight)) > 0;
            var persistedWithdraw = await _mediator.Send(new MakeVaultProposalPledgeCommand(pledge, request.BlockHeight)) > 0;

            return persistedProposal && persistedWithdraw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(VaultProposalWithdrawPledgeLog)}");

            return false;
        }
    }
}
