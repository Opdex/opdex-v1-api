using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults
{
    public class ProcessSetPendingVaultOwnershipLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessSetPendingVaultOwnershipLogCommand, bool>
    {
        private readonly ILogger<ProcessSetPendingVaultOwnershipLogCommandHandler> _logger;

        public ProcessSetPendingVaultOwnershipLogCommandHandler(IMediator mediator, ILogger<ProcessSetPendingVaultOwnershipLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessSetPendingVaultOwnershipLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await MakeTransactionLog(request.Log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(SetPendingVaultOwnershipLog)}");

                return false;
            }
        }
    }
}
