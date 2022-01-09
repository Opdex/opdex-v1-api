using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults;

public class ProcessRevokeVaultCertificateLogCommandHandler : IRequestHandler<ProcessRevokeVaultCertificateLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessRevokeVaultCertificateLogCommandHandler> _logger;

    public ProcessRevokeVaultCertificateLogCommandHandler(IMediator mediator, ILogger<ProcessRevokeVaultCertificateLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessRevokeVaultCertificateLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (vault == null) return false;

            var certs = await _mediator.Send(new RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQuery(vault.Id, request.Log.Owner));

            // Select certificates using the vestedBlock as an Id
            var certToUpdate = certs.SingleOrDefault(c => c.VestedBlock == request.Log.VestedBlock);

            if (certToUpdate == null) return false;

            if (request.BlockHeight >= certToUpdate.ModifiedBlock)
            {
                certToUpdate.Revoke(request.Log, request.BlockHeight);

                var refreshedSupply = await _mediator.Send(new MakeVaultGovernanceCommand(vault, request.BlockHeight,
                                                                                          refreshUnassignedSupply: true)) > 0;

                if (!refreshedSupply) _logger.LogError("Failure to update vault unassigned supply after revoking certificate.");

                return await _mediator.Send(new MakeVaultGovernanceCertificateCommand(certToUpdate)) > 0;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(RevokeVaultCertificateLog)}");

            return false;
        }
    }
}
