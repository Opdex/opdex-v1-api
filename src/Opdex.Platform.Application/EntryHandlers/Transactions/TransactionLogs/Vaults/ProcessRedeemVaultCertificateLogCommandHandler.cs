using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults;

public class ProcessRedeemVaultCertificateLogCommandHandler : IRequestHandler<ProcessRedeemVaultCertificateLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessRedeemVaultCertificateLogCommandHandler> _logger;

    public ProcessRedeemVaultCertificateLogCommandHandler(IMediator mediator, ILogger<ProcessRedeemVaultCertificateLogCommandHandler> logger)

    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    }

    public async Task<bool> Handle(ProcessRedeemVaultCertificateLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Log.Contract, findOrThrow: false));

            if (vault != null)
            {
                var certificates = await _mediator.Send(new RetrieveVaultCertificatesByOwnerAddressQuery(request.Log.Owner));

                // Select certificates using the vestedBlock as an Id
                var certificateToUpdate = certificates.SingleOrDefault(c => c.VestedBlock == request.Log.VestedBlock);
                if (certificateToUpdate == null) return false;

                certificateToUpdate.Redeem(request.Log, request.BlockHeight);

                return await _mediator.Send(new MakeVaultCertificateCommand(certificateToUpdate));
            }

            // Else handle certificates for the new vault contract
            var vaultGovernance = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (vaultGovernance == null) return false;

            var certs = await _mediator.Send(new RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQuery(vaultGovernance.Id, request.Log.Owner));

            // Select certificates using the vestedBlock as an Id
            var certToUpdate = certs.SingleOrDefault(c => c.VestedBlock == request.Log.VestedBlock);
            if (certToUpdate == null) return false;

            certToUpdate.Redeem(request.Log, request.BlockHeight);

            return await _mediator.Send(new MakeVaultGovernanceCertificateCommand(certToUpdate)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(RedeemVaultCertificateLog)}");

            return false;
        }
    }
}
