using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;

public class ProcessSupplyChangeLogCommandHandler : IRequestHandler<ProcessSupplyChangeLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessSupplyChangeLogCommandHandler> _logger;

    public ProcessSupplyChangeLogCommandHandler(IMediator mediator, ILogger<ProcessSupplyChangeLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessSupplyChangeLogCommand request, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract, findOrThrow: false), CancellationToken.None);
        if (token is null) return false;

        if (request.BlockHeight < token.ModifiedBlock) return true;

        token.UpdateTotalSupply(request.Log.TotalSupply, request.BlockHeight);

        var updated = await _mediator.Send(new MakeTokenCommand(token, request.BlockHeight), CancellationToken.None) > 0;
        if (updated) return true;

        _logger.LogError($"Failure processing {nameof(SupplyChangeLog)}");
        return false;
    }
}
