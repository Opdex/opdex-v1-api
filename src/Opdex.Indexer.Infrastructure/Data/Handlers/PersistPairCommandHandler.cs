using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Infrastructure.Data.Handlers
{
    public class PersistPairCommandHandler : IRequestHandler<PersistPairCommand, long>
    {
        // Todo: Insert vs update
        private static readonly string SqlCommand =
            $@"INSERT INTO pair (
                {nameof(PairEntity.Address)},
                {nameof(PairEntity.TokenId)},
                {nameof(PairEntity.ReserveToken)},
                {nameof(PairEntity.ReserveCrs)}
              ) VALUES (
                @{nameof(PairEntity.Address)},
                @{nameof(PairEntity.TokenId)},
                @{nameof(PairEntity.ReserveToken)},
                @{nameof(PairEntity.ReserveCrs)}
              );
              SELECT last_insert_rowid();";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public PersistPairCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<long> Handle(PersistPairCommand request, CancellationToken cancellationToken)
        {
            var pairEntity = _mapper.Map<PairEntity>(request.Pair);
            
            var command = DatabaseQuery.Create(SqlCommand, pairEntity, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);

            return result;
        }
    }
}