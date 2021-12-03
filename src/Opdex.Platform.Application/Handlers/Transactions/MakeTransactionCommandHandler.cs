using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Handlers.Transactions;

public class MakeTransactionCommandHandler : IRequestHandler<MakeTransactionCommand, ulong>
{
    private readonly IMediator _mediator;
    private readonly ILogger<MakeTransactionCommandHandler> _logger;

    public MakeTransactionCommandHandler(IMediator mediator, ILogger<MakeTransactionCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(MakeTransactionCommand request, CancellationToken cancellationToken)
    {
        var txId = await _mediator.Send(new PersistTransactionCommand(request.Transaction));

        request.Transaction.SetId(txId);

        await Task.WhenAll(request.Transaction.Logs.Select(async log =>
        {
            var success = await _mediator.Send(new PersistTransactionLogCommand(log));
            if (!success)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                       {
                           ["txId"] = txId,
                           ["txHash"] = request.Transaction.Hash,
                           ["LogType"] = log.LogType,
                           ["LogSortOrder"] = log.SortOrder
                       }))
                {
                    _logger.LogWarning("Transaction log unsuccessfully persisted");
                }
            }
        }));

        return txId;
    }
}