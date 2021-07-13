using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Indexer
{
    public class SelectIndexerLockQueryHandler : IRequestHandler<SelectIndexerLockQuery, IndexLock>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(IndexLockEntity.Available)},
                {nameof(IndexLockEntity.Locked)},
                {nameof(IndexLockEntity.ModifiedDate)}
            FROM index_lock
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectIndexerLockQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IndexLock> Handle(SelectIndexerLockQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, token: cancellationToken);

            var result = await _context.ExecuteFindAsync<IndexLockEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(IndexLock)} not found.");
            }

            return new IndexLock(result.Available, result.Locked, result.ModifiedDate);
        }
    }
}
