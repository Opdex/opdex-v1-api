using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions
{
    public class SelectTransactionsWithFilterQueryHandler : IRequestHandler<SelectTransactionsWithFilterQuery, List<Transaction>>
    {
        private const string TableJoins = "{TableJoins}";
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

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
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTransactionsWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<Transaction>> Handle(SelectTransactionsWithFilterQuery request, CancellationToken cancellationTransaction)
        {
            var transactionId = request.Next > 0 ? request.Next : request.Previous;

            var queryParams = new SqlParams(transactionId, request.Wallet, request.LogTypes, request.Contracts);

            var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<TransactionEntity>(query);

            return _mapper.Map<List<Transaction>>(results).ToList();
        }

        private static string QueryBuilder(SelectTransactionsWithFilterQuery request)
        {
            var whereFilter = string.Empty;
            var tableJoins = string.Empty;
            var filterContracts = request.Contracts.Any();
            var includeEvents = request.LogTypes.Any();

            if (filterContracts || includeEvents)
            {
                tableJoins += $" JOIN transaction_log tl ON tl.{nameof(TransactionLogEntity.TransactionId)} = t.{nameof(Transaction.Id)}";
            }

            var sortOperator = string.Empty;

            if (request.Next > 0 && request.Direction == SortDirectionType.ASC) sortOperator = ">";
            if (request.Next > 0 && request.Direction == SortDirectionType.DESC) sortOperator = "<";
            if (request.Previous > 0 && request.Direction == SortDirectionType.ASC) sortOperator = "<";
            if (request.Previous > 0 && request.Direction == SortDirectionType.DESC) sortOperator = ">";

            if (sortOperator.HasValue())
            {
                whereFilter = $" WHERE t.{nameof(TransactionEntity.Id)} {sortOperator} @{nameof(SqlParams.TransactionId)}";
            }

            if (request.Wallet.HasValue())
            {
                var filter = $"t.`{nameof(TransactionEntity.From)}` = @{nameof(SqlParams.Wallet)}";
                whereFilter += whereFilter.HasValue() ? $" AND {filter}" : $" WHERE {filter}";
            }

            if (request.Contracts.Any())
            {
                var filter = $"tl.{nameof(TransactionLogEntity.Contract)} IN @{nameof(SqlParams.Contracts)}";
                whereFilter += whereFilter.HasValue() ? $" AND {filter}" : $" WHERE {filter}";
            }

            if (includeEvents)
            {
                var filter = $"tl.{nameof(TransactionLogEntity.LogTypeId)} IN @{nameof(SqlParams.LogTypes)}";
                whereFilter += whereFilter.HasValue() ? $" AND {filter}" : $" WHERE {filter}";
            }

            // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
            string direction;

            if (request.Previous > 0)
            {
                direction = request.Direction == SortDirectionType.DESC ? nameof(SortDirectionType.ASC) : nameof(SortDirectionType.DESC);
            }
            else
            {
                direction = Enum.GetName(typeof(SortDirectionType), request.Direction);
            }

            var orderBy = $" GROUP BY t.{nameof(TransactionEntity.Id)} ORDER BY t.{nameof(TransactionEntity.Id)} {direction}";

            var limit = $" LIMIT {request.Limit + 1}";

            return SqlQuery
                .Replace(TableJoins, tableJoins)
                .Replace(WhereFilter, whereFilter)
                .Replace(OrderBy, orderBy)
                .Replace(Limit, limit);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long transactionId, string wallet, IEnumerable<uint> logTypes, IEnumerable<string> contracts)
            {
                TransactionId = transactionId;
                Wallet = wallet;
                LogTypes = logTypes;
                Contracts = contracts;
            }

            public long TransactionId { get; }
            public string Wallet { get; }
            public IEnumerable<uint> LogTypes { get; }
            public IEnumerable<string> Contracts { get; }
        }
    }
}
