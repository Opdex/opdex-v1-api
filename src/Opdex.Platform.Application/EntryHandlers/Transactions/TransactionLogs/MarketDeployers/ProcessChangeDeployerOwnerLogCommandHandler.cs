using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessChangeDeployerOwnerLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessChangeDeployerOwnerLogCommand, bool>
    {
        private readonly ILogger<ProcessChangeDeployerOwnerLogCommandHandler> _logger;

        public ProcessChangeDeployerOwnerLogCommandHandler(IMediator mediator, ILogger<ProcessChangeDeployerOwnerLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessChangeDeployerOwnerLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                var deployerQuery = new RetrieveDeployerByAddressQuery(request.Log.Contract, findOrThrow: true);
                var marketDeployer = await _mediator.Send(deployerQuery, CancellationToken.None);
                
                marketDeployer.SetOwner(request.Log, request.BlockHeight);

                var deployerCommand = new MakeDeployerCommand(marketDeployer);
                var deployed = await _mediator.Send(deployerCommand, CancellationToken.None);
                
                return deployed > 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ChangeDeployerOwnerLog)}");
               
                return false;
            }
        }
    }
}