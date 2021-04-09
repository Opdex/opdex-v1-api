using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;

namespace Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionLogs
{
    public class SelectLiquidityPoolCreatedLogsByIdsQueryHandler : IRequestHandler<SelectLiquidityPoolCreatedLogsByIdsQuery, IEnumerable<LiquidityPoolCreatedLog>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(LiquidityPoolCreatedLogEntity.Id)},
                {nameof(LiquidityPoolCreatedLogEntity.Token)},
                {nameof(LiquidityPoolCreatedLogEntity.Pool)}
            FROM transaction_log_liquidity_pool_created
            WHERE {nameof(LiquidityPoolCreatedLogEntity.Id)} IN @{nameof(SqlParams.TransactionLogIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectLiquidityPoolCreatedLogsByIdsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LiquidityPoolCreatedLog>> Handle(SelectLiquidityPoolCreatedLogsByIdsQuery request, CancellationToken cancellationTransaction)
        {
            var transactionLogs = request.TransactionLogs.ToDictionary(k => k.LogId);
            
            var queryParams = new SqlParams(transactionLogs.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<LiquidityPoolCreatedLogEntity>(query);

            if (!results.Any()) return Enumerable.Empty<LiquidityPoolCreatedLog>();

            var response = new List<LiquidityPoolCreatedLog>();

            foreach (var result in results)
            {
                var found = transactionLogs.TryGetValue(result.Id, out var txLog);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new LiquidityPoolCreatedLog(result.Id, txLog.TransactionId, txLog.Contract, 
                    txLog.SortOrder, result.Token, result.Pool));
            }

            return !response.Any() ? Enumerable.Empty<LiquidityPoolCreatedLog>() : response;
        }

        private sealed class SqlParams
        {
            internal SqlParams(IEnumerable<long> transactionLogIds)
            {
                TransactionLogIds = transactionLogIds;
            }

            public IEnumerable<long> TransactionLogIds { get; }
        }
    }
}