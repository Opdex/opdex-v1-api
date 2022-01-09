using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults;

public class ProcessCreateVaultCertificateLogCommandHandler : IRequestHandler<ProcessCreateVaultCertificateLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessCreateVaultCertificateLogCommandHandler> _logger;

    public ProcessCreateVaultCertificateLogCommandHandler(IMediator mediator, ILogger<ProcessCreateVaultCertificateLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessCreateVaultCertificateLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Log.Contract, findOrThrow: false));

            if (vault == null) return false;

            // Update the vault when applicable
            if (request.BlockHeight >= vault.ModifiedBlock)
            {
                var vaultId = await _mediator.Send(new MakeVaultGovernanceCommand(vault, request.BlockHeight,
                                                                                  refreshUnassignedSupply: true,
                                                                                  refreshProposedSupply: true));
                if (vaultId == 0) _logger.LogWarning($"Unexpected error updating vault governance supply by address: {vault.Address}");
            }

            // Validate that we don't already have this vault certificate inserted.
            var certs = await _mediator.Send(new RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQuery(vault.Id, request.Log.Owner));
            if (certs.Any(cert => cert.VestedBlock == request.Log.VestedBlock))
            {
                return true;
            }

            var cert = new VaultCertificate(vault.Id, request.Log.Owner, request.Log.Amount, request.Log.VestedBlock, request.BlockHeight);

            var certificateId = await _mediator.Send(new MakeVaultGovernanceCertificateCommand(cert));

            if (certificateId == 0)
            {
                _logger.LogWarning($"Unexpected error making vault certificate");
                return false;
            }

            var proposalsAtHeight = await _mediator.Send(new RetrieveVaultProposalsByModifiedBlockQuery(request.BlockHeight));
            var proposal = proposalsAtHeight.FirstOrDefault(proposal => proposal.Status == VaultProposalStatus.Complete &&
                                                                        proposal.Approved &&
                                                                        proposal.Wallet == request.Log.Owner &&
                                                                        proposal.Amount == request.Log.Amount);

            if (proposal == null)
            {
                _logger.LogWarning($"Cannot find matching proposal to certificate {certificateId}");
                return false;
            }

            var proposalCertificate = new VaultProposalCertificate(proposal.Id, certificateId, request.BlockHeight);

            var proposalCertificateId = await _mediator.Send(new MakeVaultProposalCertificateCommand(proposalCertificate));

            return proposalCertificateId > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(CreateVaultCertificateLog)}.");

            return false;
        }
    }
}
