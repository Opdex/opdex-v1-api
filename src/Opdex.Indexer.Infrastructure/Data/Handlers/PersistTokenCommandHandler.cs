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
    public class PersistTokenCommandHandler : IRequestHandler<PersistTokenCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO token (
                {nameof(TokenEntity.Address)},
                {nameof(TokenEntity.Name)},
                {nameof(TokenEntity.Symbol)},
                {nameof(TokenEntity.Decimals)},
                {nameof(TokenEntity.Sats)},
                {nameof(TokenEntity.TotalSupply)}
              ) VALUES (
                @{nameof(TokenEntity.Address)},
                @{nameof(TokenEntity.Name)},
                @{nameof(TokenEntity.Symbol)},
                @{nameof(TokenEntity.Decimals)},
                @{nameof(TokenEntity.Sats)},
                @{nameof(TokenEntity.TotalSupply)}
              );
            SELECT last_insert_rowid();";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PersistTokenCommandHandler> _logger;

        public PersistTokenCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistTokenCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<TokenEntity>(request.Token);
                var command = DatabaseQuery.Create(SqlCommand, entity, cancellationToken);
                return await _context.ExecuteScalarAsync<long>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting token {request?.Token?.Address}");
                return 0;
            }
        }
    }
}