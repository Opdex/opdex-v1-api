using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Domain.Models.Vaults;
using System.Linq;

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
            var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Log.Contract, findOrThrow: false));

            // Handle vault certificates for the current vault contract
            if (vault != null)
            {
                // Update the vault when applicable
                if (request.BlockHeight >= vault.ModifiedBlock)
                {
                    var vaultId = await _mediator.Send(new MakeVaultCommand(vault, request.BlockHeight, refreshSupply: true));
                    if (vaultId == 0) _logger.LogWarning($"Unexpected error updating vault supply by address: {vault.Address}");
                }

                // Validate that we don't already have this vault certificate inserted.
                var ownerCerts = await _mediator.Send(new RetrieveVaultCertificatesByOwnerAddressQuery(request.Log.Owner));
                if (ownerCerts.Any(cert => cert.VestedBlock == request.Log.VestedBlock))
                {
                    return true;
                }

                // Not inserted, only used to satisfy model while old vault is around for a bit longer
                const ulong proposalId = 1;
                var vaultCertificate = new VaultCertificate(vault.Id, proposalId, request.Log.Owner, request.Log.Amount, request.Log.VestedBlock, request.BlockHeight);

                return await _mediator.Send(new MakeVaultCertificateCommand(vaultCertificate));
            }

            // Else handle certificates for the new vault contract
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

            var proposalsAtHeight = await _mediator.Send(new RetrieveVaultProposalsByModifiedBlockQuery(request.BlockHeight));
            var proposal = proposalsAtHeight.First(proposal => proposal.Status == VaultProposalStatus.Complete &&
                                                               proposal.Approved &&
                                                               proposal.Wallet == request.Log.Owner &&
                                                               proposal.Amount == request.Log.Amount);

            var cert = new VaultCertificate(vaultGovernance.Id, proposal.Id, request.Log.Owner, request.Log.Amount, request.Log.VestedBlock, request.BlockHeight);

            return await _mediator.Send(new MakeVaultGovernanceCertificateCommand(cert)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(CreateVaultCertificateLog)}.");

            return false;
        }
    }
}
