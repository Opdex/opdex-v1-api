using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens
{
    public class PersistTokenCommandHandler : IRequestHandler<PersistTokenCommand, ulong>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO token (
                {nameof(TokenEntity.Address)},
                {nameof(TokenEntity.IsLpt)},
                {nameof(TokenEntity.Name)},
                {nameof(TokenEntity.Symbol)},
                {nameof(TokenEntity.Decimals)},
                {nameof(TokenEntity.Sats)},
                {nameof(TokenEntity.TotalSupply)},
                {nameof(TokenEntity.CreatedBlock)},
                {nameof(TokenEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(TokenEntity.Address)},
                @{nameof(TokenEntity.IsLpt)},
                @{nameof(TokenEntity.Name)},
                @{nameof(TokenEntity.Symbol)},
                @{nameof(TokenEntity.Decimals)},
                @{nameof(TokenEntity.Sats)},
                @{nameof(TokenEntity.TotalSupply)},
                @{nameof(TokenEntity.CreatedBlock)},
                @{nameof(TokenEntity.ModifiedBlock)}
              );
            SELECT LAST_INSERT_ID();";

        private static readonly string UpdateSqlCommand =
            $@"UPDATE token
                SET
                    {nameof(TokenEntity.TotalSupply)} = @{nameof(TokenEntity.TotalSupply)},
                    {nameof(TokenEntity.ModifiedBlock)} = @{nameof(TokenEntity.ModifiedBlock)}
                WHERE {nameof(TokenEntity.Id)} = @{nameof(TokenEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PersistTokenCommandHandler(IDbContext context, IMapper mapper,
            ILogger<PersistTokenCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ulong> Handle(PersistTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<TokenEntity>(request.Token);

                var isUpdate = entity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

                var command = DatabaseQuery.Create(sql, entity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<ulong>(command);

                return isUpdate ? entity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting token {request?.Token?.Address}");

                return 0;
            }
        }
    }
}
