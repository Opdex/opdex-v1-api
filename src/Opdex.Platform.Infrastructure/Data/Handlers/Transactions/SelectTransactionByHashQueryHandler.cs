using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions;

public class SelectTransactionByHashQueryHandler : IRequestHandler<SelectTransactionByHashQuery, Transaction>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(TransactionEntity.Id)},
                `{nameof(TransactionEntity.To)}`,
                `{nameof(TransactionEntity.From)}`,
                {nameof(TransactionEntity.Hash)},
                {nameof(TransactionEntity.Block)},
                {nameof(TransactionEntity.GasUsed)},
                {nameof(TransactionEntity.NewContractAddress)},
                {nameof(TransactionEntity.Success)},
                {nameof(TransactionEntity.Error)}
            FROM transaction
            WHERE {nameof(TransactionEntity.Hash)} = @{nameof(SqlParams.Hash)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTransactionByHashQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Transaction> Handle(SelectTransactionByHashQuery request, CancellationToken cancellationTransaction)
    {
        var queryParams = new SqlParams(request.Hash);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

        var result = await _context.ExecuteFindAsync<TransactionEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(Transaction)} not found.");
        }

        return result == null ? null : _mapper.Map<Transaction>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(Sha256 hash)
        {
            Hash = hash;
        }

        public Sha256 Hash { get; }
    }
}
