using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions;

public class SelectTransactionsForSnapshotRewindQueryHandler
    : IRequestHandler<SelectTransactionsForSnapshotRewindQuery, IEnumerable<Transaction>>
{
    private const string Split = "Split";

    private static readonly string SqlQuery =
        @$"SELECT
                t.{nameof(TransactionEntity.Id)},
                t.`{nameof(TransactionEntity.To)}`,
                t.`{nameof(TransactionEntity.From)}`,
                t.{nameof(TransactionEntity.Hash)},
                t.{nameof(TransactionEntity.Block)},
                t.{nameof(TransactionEntity.GasUsed)},
                t.{nameof(TransactionEntity.NewContractAddress)},
                t.{nameof(TransactionEntity.Success)},
                true as {Split},
                tl.{nameof(TransactionLogEntity.Id)},
                tl.{nameof(TransactionLogEntity.TransactionId)},
                tl.{nameof(TransactionLogEntity.LogTypeId)},
                tl.{nameof(TransactionLogEntity.SortOrder)},
                tl.{nameof(TransactionLogEntity.Contract)},
                tl.{nameof(TransactionLogEntity.Details)}
            FROM transaction t
            LEFT JOIN block b
                ON t.{nameof(TransactionEntity.Block)} = b.{nameof(BlockEntity.Height)}
            LEFT JOIN transaction_log tl
                ON t.{nameof(TransactionEntity.Id)} = tl.{nameof(TransactionLogEntity.TransactionId)}
            WHERE b.{nameof(BlockEntity.MedianTime)} >= @{nameof(SqlParams.DateTime)}
            ORDER BY t.{nameof(TransactionEntity.Id)} ASC;";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTransactionsForSnapshotRewindQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<Transaction>> Handle(SelectTransactionsForSnapshotRewindQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.DateTime), cancellationToken);

        var transactions = new Dictionary<ulong, Transaction>();

        await _context.ExecuteQueryAsync<TransactionEntity, TransactionLogEntity, Transaction>(query, (transaction, log) =>
        {
            var exists = transactions.TryGetValue(transaction.Id, out Transaction tx);
            if (!exists) tx = _mapper.Map<Transaction>(transaction);

            if (log != null)
            {
                tx.SetLog(_mapper.Map<TransactionLog>(log));
            }

            transactions[tx.Id] = tx;

            return tx;
        }, splitOn: Split);

        return transactions.Values;
    }

    private sealed class SqlParams
    {
        internal SqlParams(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        public DateTime DateTime { get; }
    }
}