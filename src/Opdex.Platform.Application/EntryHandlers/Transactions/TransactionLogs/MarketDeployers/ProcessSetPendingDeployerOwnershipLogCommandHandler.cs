using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers;

public class ProcessSetPendingDeployerOwnershipLogCommandHandler : IRequestHandler<ProcessSetPendingDeployerOwnershipLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessSetPendingDeployerOwnershipLogCommandHandler> _logger;

    public ProcessSetPendingDeployerOwnershipLogCommandHandler(IMediator mediator, ILogger<ProcessSetPendingDeployerOwnershipLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessSetPendingDeployerOwnershipLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var deployer = await _mediator.Send(new RetrieveDeployerByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (deployer == null) return false;

            if (request.BlockHeight < deployer.ModifiedBlock)
            {
                return true;
            }

            deployer.SetPendingOwnership(request.Log, request.BlockHeight);

            return await _mediator.Send(new MakeDeployerCommand(deployer, request.BlockHeight)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(SetPendingDeployerOwnershipLog)}");

            return false;
        }
    }
}