using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.VaultGovernances;

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
            var vaultGovernance = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Log.Contract, findOrThrow: false));

            if (vaultGovernance == null) return false;

            // Update the vault when applicable
            if (request.BlockHeight >= vaultGovernance.ModifiedBlock)
            {
                var vaultId = await _mediator.Send(new MakeVaultGovernanceCommand(vaultGovernance, request.BlockHeight,
                                                                                  refreshUnassignedSupply: true,
                                                                                  refreshProposedSupply: true));
                if (vaultId == 0) _logger.LogWarning($"Unexpected error updating vault governance supply by address: {vaultGovernance.Address}");
            }

            // Validate that we don't already have this vault certificate inserted.
            var certs = await _mediator.Send(new RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQuery(vaultGovernance.Id, request.Log.Owner));
            if (certs.Any(cert => cert.VestedBlock == request.Log.VestedBlock))
            {
                return true;
            }

            var cert = new VaultCertificate(vaultGovernance.Id, request.Log.Owner, request.Log.Amount, request.Log.VestedBlock, request.BlockHeight);

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
