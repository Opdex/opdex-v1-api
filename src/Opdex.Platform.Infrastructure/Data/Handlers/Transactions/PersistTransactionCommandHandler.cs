using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions;

public class PersistTransactionCommandHandler : IRequestHandler<PersistTransactionCommand, ulong>
{
    private static readonly string SqlCommand =
        $@"INSERT INTO transaction (
                `{nameof(TransactionEntity.From)}`,
                `{nameof(TransactionEntity.To)}`,
                {nameof(TransactionEntity.NewContractAddress)},
                {nameof(TransactionEntity.Hash)},
                {nameof(TransactionEntity.GasUsed)},
                {nameof(TransactionEntity.Success)},
                {nameof(TransactionEntity.Error)},
                {nameof(TransactionEntity.Block)}
              ) VALUES (
                @{nameof(TransactionEntity.From)},
                @{nameof(TransactionEntity.To)},
                @{nameof(TransactionEntity.NewContractAddress)},
                @{nameof(TransactionEntity.Hash)},
                @{nameof(TransactionEntity.GasUsed)},
                @{nameof(TransactionEntity.Success)},
                @{nameof(TransactionEntity.Error)},
                @{nameof(TransactionEntity.Block)}
              );
              SELECT LAST_INSERT_ID();";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistTransactionCommandHandler(IDbContext context, IMapper mapper,
                                            ILogger<PersistTransactionCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var transactionEntity = _mapper.Map<TransactionEntity>(request.Transaction);

            var command = DatabaseQuery.Create(SqlCommand, transactionEntity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            if (result == 0)
            {
                throw new Exception("Error persisting transaction.");
            }

            return result;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "Hash", request.Transaction.Hash },
                { "BlockHeight", request.Transaction.BlockHeight },
            }))
            {
                _logger.LogError(ex, $"Unable to persist transaction.");
            }

            return 0;
        }
    }
}
