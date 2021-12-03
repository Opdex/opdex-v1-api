using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningGovernances;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningGovernances;

public class ProcessNominationLogCommandHandler : IRequestHandler<ProcessNominationLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessNominationLogCommandHandler> _logger;

    public ProcessNominationLogCommandHandler(IMediator mediator, ILogger<ProcessNominationLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessNominationLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _mediator.Send(new CreateMiningGovernanceNominationsCommand(request.Log.Contract, request.BlockHeight));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(NominationLog)}");

            return false;
        }
    }
}