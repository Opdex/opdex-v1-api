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
    public class SelectReservesLogsByIdsQueryHandler : IRequestHandler<SelectReservesLogsByIdsQuery, IEnumerable<ReservesLog>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(ReservesLogEntity.Id)},
                {nameof(ReservesLogEntity.ReserveCrs)},
                {nameof(ReservesLogEntity.ReserveSrc)}
            FROM transaction_log_reserves
            WHERE {nameof(ReservesLogEntity.Id)} IN @{nameof(SqlParams.TransactionLogIds)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectReservesLogsByIdsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ReservesLog>> Handle(SelectReservesLogsByIdsQuery request, CancellationToken cancellationTransaction)
        {
            var transactionLogs = request.TransactionLogs.ToDictionary(k => k.LogId);
            
            var queryParams = new SqlParams(transactionLogs.Keys);
            
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<ReservesLogEntity>(query);

            if (!results.Any()) return Enumerable.Empty<ReservesLog>();

            var response = new List<ReservesLog>();

            foreach (var result in results)
            {
                var found = transactionLogs.TryGetValue(result.Id, out var txLog);
                if (!found)
                {
                    continue;
                }
                
                response.Add(new ReservesLog(result.Id, txLog.TransactionId, txLog.Contract, 
                    txLog.SortOrder, result.ReserveCrs, result.ReserveSrc));
            }

            return !response.Any() ? Enumerable.Empty<ReservesLog>() : response;
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