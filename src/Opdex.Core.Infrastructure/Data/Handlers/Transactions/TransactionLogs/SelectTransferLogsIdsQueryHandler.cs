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
    public class SelectTransferLogsByIdsQueryHandler : IRequestHandler<SelectTransferLogsByIdsQuery, IEnumerable<TransferLog>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(TransferLogEntity.Id)},
                `{nameof(TransferLogEntity.To)}`,
                `{nameof(TransferLogEntity.From)}`,
                {nameof(TransferLogEntity.Amount)}
            FROM transaction_log_transfer
            WHERE {nameof(TransferLogEntity.Id)} IN @{nameof(SqlParams.TransactionLogIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTransferLogsByIdsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TransferLog>> Handle(SelectTransferLogsByIdsQuery request, CancellationToken cancellationTransaction)
        {
            var transactionLogs = request.TransactionLogs.ToDictionary(k => k.LogId);
            
            var queryParams = new SqlParams(transactionLogs.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<TransferLogEntity>(query);

            if (!results.Any()) return Enumerable.Empty<TransferLog>();

            var response = new List<TransferLog>();

            foreach (var result in results)
            {
                var found = transactionLogs.TryGetValue(result.Id, out var txLog);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new TransferLog(result.Id, txLog.TransactionId, txLog.Contract, 
                    txLog.SortOrder, result.From, result.To, result.Amount));
            }

            return !response.Any() ? Enumerable.Empty<TransferLog>() : response;
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