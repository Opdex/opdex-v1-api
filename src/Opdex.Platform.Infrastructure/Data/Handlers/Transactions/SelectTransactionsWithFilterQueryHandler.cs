using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions;

public class SelectTransactionsWithFilterQueryHandler : IRequestHandler<SelectTransactionsWithFilterQuery, IEnumerable<Transaction>>
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
                t.{nameof(TransactionEntity.Error)},
                t.{nameof(TransactionEntity.NewContractAddress)}
            FROM transaction t
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string SortDirection = "{SortDirection}";

    private static readonly string PagingBackwardQuery =
        @$"SELECT * FROM ({InnerQuery}) r ORDER BY r.{nameof(TransactionEntity.Id)} {SortDirection};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTransactionsWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<Transaction>> Handle(SelectTransactionsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var logTypes = _mapper.Map<IEnumerable<TransactionLogType>>(request.Cursor.EventTypes.Distinct()).Cast<uint>();

        var queryParams = new SqlParams(request.Cursor.Pointer, request.Cursor.Wallet, logTypes, request.Cursor.Contracts);

        var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<TransactionEntity>(query);

        return _mapper.Map<IEnumerable<Transaction>>(results);
    }

    private static string QueryBuilder(SelectTransactionsWithFilterQuery request)
    {
        var whereFilter = string.Empty;
        var tableJoins = string.Empty;
        var filterContracts = request.Cursor.Contracts.Any();
        var includeEvents = request.Cursor.EventTypes.Any();

        if (filterContracts || includeEvents)
        {
            tableJoins += $" JOIN transaction_log tl ON tl.{nameof(TransactionLogEntity.TransactionId)} = t.{nameof(Transaction.Id)}";
        }

        if (!request.Cursor.IsFirstRequest)
        {
            var sortOperator = string.Empty;

            // going forward in ascending order, use greater than
            if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = ">";

            // going forward in descending order, use less than or equal to
            if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = "<";

            // going backward in ascending order, use less than
            if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = "<";

            // going backward in descending order, use greater than
            if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = ">";

            whereFilter = $" WHERE t.{nameof(TransactionEntity.Id)} {sortOperator} @{nameof(SqlParams.TransactionId)}";
        }

        if (request.Cursor.Wallet != Address.Empty)
        {
            var filter = $"t.`{nameof(TransactionEntity.From)}` = @{nameof(SqlParams.Wallet)}";
            whereFilter += whereFilter.HasValue() ? $" AND {filter}" : $" WHERE {filter}";
        }

        if (filterContracts)
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

        if (request.Cursor.PagingDirection == PagingDirection.Backward)
        {
            direction = request.Cursor.SortDirection == SortDirectionType.DESC ? nameof(SortDirectionType.ASC) : nameof(SortDirectionType.DESC);
        }
        else
        {
            direction = Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection);
        }

        var orderBy = $" GROUP BY t.{nameof(TransactionEntity.Id)} ORDER BY t.{nameof(TransactionEntity.Id)} {direction}";

        var limit = $" LIMIT {request.Cursor.Limit + 1}";

        var query = SqlQuery.Replace(TableJoins, tableJoins)
            .Replace(WhereFilter, whereFilter)
            .Replace(OrderBy, orderBy)
            .Replace(Limit, limit);

        if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";

        // re-sort back into requested order
        return PagingBackwardQuery.Replace(InnerQuery, query)
            .Replace(SortDirection, Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection));
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong transactionId, Address wallet, IEnumerable<uint> logTypes, IEnumerable<Address> contracts)
        {
            TransactionId = transactionId;
            Wallet = wallet;
            LogTypes = logTypes;
            Contracts = contracts.Select(contract => contract.ToString());
        }

        public ulong TransactionId { get; }
        public Address Wallet { get; }
        public IEnumerable<uint> LogTypes { get; }
        public IEnumerable<string> Contracts { get; }
    }
}
