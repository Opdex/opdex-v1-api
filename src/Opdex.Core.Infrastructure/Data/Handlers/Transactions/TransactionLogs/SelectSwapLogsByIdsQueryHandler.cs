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
    public class SelectSwapLogsByIdsQueryHandler : IRequestHandler<SelectSwapLogsByIdsQuery, IEnumerable<SwapLog>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(SwapLogEntity.Id)},
                {nameof(SwapLogEntity.Sender)},
                `{nameof(SwapLogEntity.To)}`,
                {nameof(SwapLogEntity.AmountCrsIn)},
                {nameof(SwapLogEntity.AmountCrsOut)},
                {nameof(SwapLogEntity.AmountSrcIn)},
                {nameof(SwapLogEntity.AmountSrcOut)}
            FROM transaction_log_swap
            WHERE {nameof(SwapLogEntity.Id)} IN @{nameof(SqlParams.TransactionLogIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectSwapLogsByIdsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<SwapLog>> Handle(SelectSwapLogsByIdsQuery request, CancellationToken cancellationTransaction)
        {
            var transactionLogs = request.TransactionLogs.ToDictionary(k => k.LogId);
            
            var queryParams = new SqlParams(transactionLogs.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<SwapLogEntity>(query);

            if (!results.Any()) return Enumerable.Empty<SwapLog>();

            var response = new List<SwapLog>();

            foreach (var result in results)
            {
                var found = transactionLogs.TryGetValue(result.Id, out var txLog);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new SwapLog(result.Id, txLog.TransactionId, txLog.Contract, txLog.SortOrder, result.Sender,
                    result.To, result.AmountCrsIn, result.AmountCrsOut, result.AmountSrcIn, result.AmountSrcOut));
            }

            return !response.Any() ? Enumerable.Empty<SwapLog>() : response;
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