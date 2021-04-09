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
    public class SelectBurnLogsByIdsQueryHandler : IRequestHandler<SelectBurnLogsByIdsQuery, IEnumerable<BurnLog>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(BurnLogEntity.Id)},
                {nameof(BurnLogEntity.Sender)},
                `{nameof(BurnLogEntity.To)}`,
                {nameof(BurnLogEntity.AmountCrs)},
                {nameof(BurnLogEntity.AmountSrc)}
            FROM transaction_log_burn
            WHERE {nameof(BurnLogEntity.Id)} IN @{nameof(SqlParams.TransactionLogIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectBurnLogsByIdsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<BurnLog>> Handle(SelectBurnLogsByIdsQuery request, CancellationToken cancellationTransaction)
        {
            var transactionLogs = request.TransactionLogs.ToDictionary(k => k.LogId);
            
            var queryParams = new SqlParams(transactionLogs.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<BurnLogEntity>(query);

            if (!results.Any()) return Enumerable.Empty<BurnLog>();

            var response = new List<BurnLog>();

            foreach (var result in results)
            {
                var found = transactionLogs.TryGetValue(result.Id, out var txLog);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new BurnLog(result.Id, txLog.TransactionId, txLog.Contract, 
                    txLog.SortOrder, result.Sender, result.To, result.AmountCrs, result.AmountSrc));
            }

            return !response.Any() ? Enumerable.Empty<BurnLog>() : response;
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