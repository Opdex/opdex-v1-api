using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents;

namespace Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionEvents
{
    public class SelectMintEventsByTransactionIdQueryHandler : IRequestHandler<SelectMintEventsByTransactionIdQuery, IEnumerable<MintEvent>>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(MintEventEntity.Id)},
                {nameof(MintEventEntity.TransactionId)},
                {nameof(MintEventEntity.Address)},
                {nameof(MintEventEntity.SortOrder)},
                {nameof(MintEventEntity.Sender)},
                {nameof(MintEventEntity.AmountCrs)},
                {nameof(MintEventEntity.AmountSrc)}
            FROM transaction_event_mint
            WHERE {nameof(MintEventEntity.TransactionId)} = @{nameof(SqlParams.TransactionId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectMintEventsByTransactionIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<MintEvent>> Handle(SelectMintEventsByTransactionIdQuery request, CancellationToken cancellationTransaction)
        {
            var queryParams = new SqlParams(request.TransactionId);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationTransaction);

            var result = await _context.ExecuteQueryAsync<MintEventEntity>(query);

            return !result.Any() ? Enumerable.Empty<MintEvent>() : _mapper.Map<IEnumerable<MintEvent>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long transactionId)
            {
                TransactionId = transactionId;
            }

            public long TransactionId { get; }
        }
    }
}