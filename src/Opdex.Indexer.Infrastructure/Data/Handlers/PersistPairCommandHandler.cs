using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
                {nameof(PairEntity.ReserveSrc)},
                {nameof(PairEntity.ReserveCrs)},
                {nameof(PairEntity.CreatedDate)}
              ) VALUES (
                @{nameof(PairEntity.Address)},
                @{nameof(PairEntity.TokenId)},
                @{nameof(PairEntity.ReserveSrc)},
                @{nameof(PairEntity.ReserveCrs)},
                UTC_TIMESTAMP()
              );
              SELECT last_insert_rowid();";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistPairCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistPairCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistPairCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var pairEntity = _mapper.Map<PairEntity>(request.Pair);
            
                var command = DatabaseQuery.Create(SqlCommand, pairEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);

                return result;
            }
            catch (Exception)
            {
                _logger.LogError($"Unable to persist {request.Pair}");
                return 0;
            }
        }
    }
}