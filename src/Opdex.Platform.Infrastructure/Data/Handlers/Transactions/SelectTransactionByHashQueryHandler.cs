using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions
{
    public class SelectTransactionByHashQueryHandler : IRequestHandler<SelectTransactionByHashQuery, Transaction>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(TransactionEntity.Id)},
                {nameof(TransactionEntity.Hash)},
                {nameof(TransactionEntity.Block)},
                {nameof(TransactionEntity.GasUsed)},
                `{nameof(TransactionEntity.To)}`,
                `{nameof(TransactionEntity.From)}`
            FROM transaction
            WHERE {nameof(TransactionEntity.Hash)} = @{nameof(SqlParams.Hash)};";

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

            if (result == null)
            {
                throw new NotFoundException($"{nameof(TransactionEntity)} with hash {request.Hash} was not found.");
            }

            return _mapper.Map<Transaction>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(string hash)
            {
                Hash = hash;
            }

            public string Hash { get; }
        }
    }
}