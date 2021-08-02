using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults
{
    public class ProcessClaimPendingVaultOwnershipLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessClaimPendingVaultOwnershipLogCommand, bool>
    {
        private readonly ILogger<ProcessClaimPendingVaultOwnershipLogCommandHandler> _logger;

        public ProcessClaimPendingVaultOwnershipLogCommandHandler(IMediator mediator, ILogger<ProcessClaimPendingVaultOwnershipLogCommandHandler> logger)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessClaimPendingVaultOwnershipLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Log.Contract, findOrThrow: true));

                vault.SetOwner(request.Log, request.BlockHeight);

                var vaultId = await _mediator.Send( new MakeVaultCommand(vault));

                return vaultId > 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ClaimPendingVaultOwnershipLog)}");

                return false;
            }
        }
    }
}
