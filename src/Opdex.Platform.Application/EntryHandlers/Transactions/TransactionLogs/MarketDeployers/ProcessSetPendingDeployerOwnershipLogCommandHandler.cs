using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessSetPendingDeployerOwnershipLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessSetPendingDeployerOwnershipLogCommand, bool>
    {
        private readonly ILogger<ProcessSetPendingDeployerOwnershipLogCommandHandler> _logger;

        public ProcessSetPendingDeployerOwnershipLogCommandHandler(IMediator mediator, ILogger<ProcessSetPendingDeployerOwnershipLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessSetPendingDeployerOwnershipLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await MakeTransactionLog(request.Log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(SetPendingDeployerOwnershipLog)}");

                return false;
            }
        }
    }
}
