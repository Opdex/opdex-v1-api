using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System.Linq;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults
{
    public class ProcessCreateVaultCertificateLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessCreateVaultCertificateLogCommand, bool>
    {
        private readonly ILogger<ProcessCreateVaultCertificateLogCommandHandler> _logger;

        public ProcessCreateVaultCertificateLogCommandHandler(IMediator mediator, ILogger<ProcessCreateVaultCertificateLogCommandHandler> logger)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCreateVaultCertificateLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Log.Contract, findOrThrow: true));

                // Update the vault when applicable
                if (request.BlockHeight >= vault.ModifiedBlock)
                {
                    var vaultId = await _mediator.Send(new MakeVaultCommand(vault, request.BlockHeight, refreshSupply: true));
                    if (vaultId <= 0) _logger.LogError($"Unexpected error updating vault supply by address: {vault.Address}");
                }

                // Validate that we don't already have this vault certificate inserted.
                var ownerCerts = await _mediator.Send(new RetrieveVaultCertificatesByOwnerAddressQuery(request.Log.Owner));
                if (ownerCerts.Any(cert => cert.VestedBlock == request.Log.VestedBlock))
                {
                    return true;
                }

                // Insert if we reach here
                var vaultCertificate = new VaultCertificate(vault.Id, request.Log.Owner, request.Log.Amount, request.Log.VestedBlock, request.BlockHeight);
                return await _mediator.Send(new MakeVaultCertificateCommand(vaultCertificate));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CreateVaultCertificateLog)}.");

                return false;
            }
        }
    }
}
