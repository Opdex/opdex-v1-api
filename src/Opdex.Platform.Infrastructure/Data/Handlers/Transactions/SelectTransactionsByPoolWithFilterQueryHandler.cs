using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions
{
    public class SelectTransactionsByPoolWithFilterQueryHandler 
        : IRequestHandler<SelectTransactionsByPoolWithFilterQuery, IEnumerable<Transaction>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                t.{nameof(TransactionEntity.Id)},
                t.{nameof(TransactionEntity.Hash)},
                t.{nameof(TransactionEntity.Block)},
                t.{nameof(TransactionEntity.GasUsed)},
                t.`{nameof(TransactionEntity.To)}`,
                t.`{nameof(TransactionEntity.From)}`,
                t.{nameof(TransactionEntity.Success)},
                t.{nameof(TransactionEntity.NewContractAddress)}
            FROM transaction t
            LEFT JOIN transaction_log tl 
                ON tl.{nameof(TransactionLogEntity.TransactionId)} = t.{nameof(TransactionEntity.Id)} 
            LEFT JOIN pool_liquidity p 
                ON p.{nameof(LiquidityPoolEntity.Address)} = tl.{nameof(TransactionLogEntity.Contract)}
            WHERE tl.{nameof(TransactionLogEntity.Id)} IS NOT NULL
                AND p.{nameof(LiquidityPoolEntity.Address)} = @{nameof(SqlParams.PoolAddress)}
                AND tl.{nameof(TransactionLogEntity.LogTypeId)} IN @{nameof(SqlParams.LogTypes)}
            ORDER BY t.{nameof(TransactionEntity.Id)} DESC
            LIMIT 10;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTransactionsByPoolWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Transaction>> Handle(SelectTransactionsByPoolWithFilterQuery request,
            CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.PoolAddress, request.LogTypes);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteQueryAsync<TransactionEntity>(query);

            return _mapper.Map<IEnumerable<Transaction>>(result.GroupBy(p => p.Hash).Select(g => g.First()));
        }

        private sealed class SqlParams
        {
            internal SqlParams(string poolAddress, IEnumerable<int> logTypes)
            {
                PoolAddress = poolAddress;
                LogTypes = logTypes;
            }

            public string PoolAddress { get; }
            public IEnumerable<int> LogTypes { get; }
        }
    }
}