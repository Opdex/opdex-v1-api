using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Wrapped;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;

public class ProcessOwnershipTransferredLogCommandHandler : IRequestHandler<ProcessOwnershipTransferredLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessOwnershipTransferredLogCommandHandler> _logger;

    public ProcessOwnershipTransferredLogCommandHandler(IMediator mediator, ILogger<ProcessOwnershipTransferredLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessOwnershipTransferredLogCommand request, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract, findOrThrow: false), CancellationToken.None);
        if (token is null) return false;

        TokenWrapped wrappedToken;
        try
        {
            wrappedToken = await _mediator.Send(new RetrieveTokenWrappedByTokenIdQuery(token.Id), CancellationToken.None);
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(OwnershipTransferredLog)}");
            return false;
        }

        if (request.BlockHeight < wrappedToken.ModifiedBlock) return true;

        wrappedToken.SetOwner(request.Log.To, request.BlockHeight);

        var updated = await _mediator.Send(new MakeTokenWrappedCommand(wrappedToken), CancellationToken.None) > 0;
        if (updated) return true;

        _logger.LogError($"Failure processing {nameof(OwnershipTransferredLog)}");
        return false;
    }
}
