using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults
{
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
                var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Log.Contract, findOrThrow: true));

                if (request.BlockHeight >= vault.ModifiedBlock)
                {
                    var vaultId = await _mediator.Send(new MakeVaultCommand(vault, request.BlockHeight, refreshSupply: true));
                    if (vaultId <= 0) _logger.LogError($"Unexpected error updating vault supply by address: {vault.Address}");
                }

                var certificates = await _mediator.Send(new RetrieveVaultCertificatesByOwnerAddressQuery(request.Log.Owner));

                // Select certificates using the vestedBlock as an Id
                var certificateToUpdate = certificates.Single(c => c.VestedBlock == request.Log.VestedBlock);

                if (request.BlockHeight >= certificateToUpdate.ModifiedBlock)
                {
                    certificateToUpdate.Revoke(request.Log, request.BlockHeight);

                    return await _mediator.Send(new MakeVaultCertificateCommand(certificateToUpdate));
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
}
