using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults
{
    public class ProcessRevokeVaultCertificateLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessRevokeVaultCertificateLogCommand, bool>
    {
        private readonly ILogger<ProcessRevokeVaultCertificateLogCommandHandler> _logger;

        public ProcessRevokeVaultCertificateLogCommandHandler(IMediator mediator, ILogger<ProcessRevokeVaultCertificateLogCommandHandler> logger)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessRevokeVaultCertificateLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var vault = await _mediator.Send(new RetrieveVaultQuery(findOrThrow: true));

                if (request.BlockHeight > vault.ModifiedBlock)
                {
                    var totalSupply = await _mediator.Send(new CallCirrusGetVaultTotalSupplyQuery(vault.Address, request.BlockHeight));

                    vault.SetUnassignedSupply(totalSupply, request.BlockHeight);

                    var vaultUpdates = await _mediator.Send(new MakeVaultCommand(vault));
                    if (vaultUpdates == 0) return false;

                    var certificates = await _mediator.Send(new RetrieveVaultCertificatesByOwnerAddressQuery(request.Log.Owner));

                    // Todo: Maybe create a specific query for this and a unique index on (owner, vestedBlock)
                    var certificateToUpdate = certificates.Single(c => c.VestedBlock == request.Log.VestedBlock);

                    certificateToUpdate.Revoke(request.Log, request.BlockHeight);

                    var certificateUpdated = await _mediator.Send(new MakeVaultCertificateCommand(certificateToUpdate));
                    return certificateUpdated;
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
