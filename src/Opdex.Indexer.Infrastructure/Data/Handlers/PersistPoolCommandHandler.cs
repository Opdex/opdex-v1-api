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
    public class PersistPoolCommandHandler : IRequestHandler<PersistPoolCommand, long>
    {
        // Todo: Insert vs update
        private static readonly string SqlCommand =
            $@"INSERT INTO pool (
                {nameof(PoolEntity.Address)},
                {nameof(PoolEntity.TokenId)},
                {nameof(PoolEntity.ReserveSrc)},
                {nameof(PoolEntity.ReserveCrs)},
                {nameof(PoolEntity.CreatedDate)}
              ) VALUES (
                @{nameof(PoolEntity.Address)},
                @{nameof(PoolEntity.TokenId)},
                @{nameof(PoolEntity.ReserveSrc)},
                @{nameof(PoolEntity.ReserveCrs)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistPoolCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistPoolCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistPoolCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var poolEntity = _mapper.Map<PoolEntity>(request.Pool);
            
                var command = DatabaseQuery.Create(SqlCommand, poolEntity, cancellationToken);
            
                return await _context.ExecuteScalarAsync<long>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.Pool}");
                return 0;
            }
        }
    }
}