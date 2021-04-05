using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
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
                t.`{nameof(TransactionEntity.From)}`
            FROM transaction t
            LEFT JOIN transaction_event_summary tes 
                ON tes.{nameof(TransactionEventSummaryEntity.TransactionId)} = t.{nameof(TransactionEntity.Id)} 
            LEFT JOIN pool p 
                ON p.{nameof(PoolEntity.Address)} = tes.{nameof(TransactionEventSummaryEntity.Contract)}
            WHERE tes.{nameof(TransactionEventSummaryEntity.Id)} IS NOT NULL
                AND {nameof(PoolEntity.Address)} = @{nameof(SqlParams.PoolAddress)}
                AND {nameof(TransactionEventSummaryEntity.EventTypeId)} IN @{nameof(SqlParams.EventTypes)}
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
            var queryParams = new SqlParams(request.PoolAddress, request.EventTypes);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteQueryAsync<TransactionEntity>(query);

            return _mapper.Map<IEnumerable<Transaction>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(string poolAddress, IEnumerable<int> eventTypes)
            {
                PoolAddress = poolAddress;
                EventTypes = eventTypes;
            }

            public string PoolAddress { get; }
            public IEnumerable<int> EventTypes { get; }
        }
    }
}