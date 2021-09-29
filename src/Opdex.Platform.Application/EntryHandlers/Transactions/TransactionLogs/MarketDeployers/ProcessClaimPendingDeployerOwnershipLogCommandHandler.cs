using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessClaimPendingDeployerOwnershipLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessClaimPendingDeployerOwnershipLogCommand, bool>
    {
        private readonly ILogger<ProcessClaimPendingDeployerOwnershipLogCommandHandler> _logger;

        public ProcessClaimPendingDeployerOwnershipLogCommandHandler(IMediator mediator, ILogger<ProcessClaimPendingDeployerOwnershipLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessClaimPendingDeployerOwnershipLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);

                if (!persisted) return false;

                var deployer = await _mediator.Send(new CreateDeployerCommand(request.Log.Contract, Address.Empty, request.Log.To, request.BlockHeight));

                return deployer > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ClaimPendingDeployerOwnershipLog)}");

                return false;
            }
        }
    }
}
