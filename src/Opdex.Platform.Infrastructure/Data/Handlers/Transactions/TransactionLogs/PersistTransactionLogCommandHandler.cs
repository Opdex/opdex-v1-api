using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions.TransactionLogs;

public class PersistTransactionLogCommandHandler : IRequestHandler<PersistTransactionLogCommand, bool>
{
    private static readonly string SqlCommand =
        $@"INSERT INTO transaction_log (
                {nameof(TransactionLogEntity.TransactionId)},
                {nameof(TransactionLogEntity.LogTypeId)},
                {nameof(TransactionLogEntity.Contract)},
                {nameof(TransactionLogEntity.SortOrder)},
                {nameof(TransactionLogEntity.Details)}
              ) VALUES (
                @{nameof(TransactionLogEntity.TransactionId)},
                @{nameof(TransactionLogEntity.LogTypeId)},
                @{nameof(TransactionLogEntity.Contract)},
                @{nameof(TransactionLogEntity.SortOrder)},
                @{nameof(TransactionLogEntity.Details)}
              );".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistTransactionLogCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistTransactionLogCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(PersistTransactionLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var approvalLogEntity = _mapper.Map<TransactionLogEntity>(request.TransactionLog);

            var command = DatabaseQuery.Create(SqlCommand, approvalLogEntity, cancellationToken);

            var result = await _context.ExecuteCommandAsync(command);

            return result > 0;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "Contract", request.TransactionLog.Contract },
                { "Hash", request.TransactionLog.TransactionId },
                { "LogType", request.TransactionLog.LogType },
                { "SortOrder", request.TransactionLog.SortOrder }
            }))
            {
                _logger.LogError(ex, $"Unable to persist transaction log.");
            }
            return false;
        }
    }
}