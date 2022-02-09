using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens.Wrapped;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Wrapped;

public class PersistTokenWrappedCommandHandler : IRequestHandler<PersistTokenWrappedCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO token_wrapped (
                {nameof(TokenWrappedEntity.TokenId)},
                {nameof(TokenWrappedEntity.Owner)},
                {nameof(TokenWrappedEntity.NativeChainTypeId)},
                {nameof(TokenWrappedEntity.NativeAddress)},
s                {nameof(TokenWrappedEntity.Validated)},
                {nameof(TokenWrappedEntity.CreatedBlock)},
                {nameof(TokenWrappedEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(TokenWrappedEntity.TokenId)},
                @{nameof(TokenWrappedEntity.Owner)},
                @{nameof(TokenWrappedEntity.NativeChainTypeId)},
                @{nameof(TokenWrappedEntity.NativeAddress)},
                @{nameof(TokenWrappedEntity.Validated)},
                @{nameof(TokenWrappedEntity.CreatedBlock)},
                @{nameof(TokenWrappedEntity.ModifiedBlock)}
              );
            SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE token_wrapped
                SET
                    {nameof(TokenWrappedEntity.Owner)} = @{nameof(TokenWrappedEntity.Owner)},
                    {nameof(TokenWrappedEntity.ModifiedBlock)} = @{nameof(TokenWrappedEntity.ModifiedBlock)}
                WHERE {nameof(TokenWrappedEntity.Id)} = @{nameof(TokenWrappedEntity.Id)};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistTokenWrappedCommandHandler(IDbContext context, IMapper mapper,
        ILogger<PersistTokenWrappedCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistTokenWrappedCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<TokenWrappedEntity>(request.Wrapped);

            var isUpdate = entity.Id >= 1;
            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return isUpdate ? entity.Id : result;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
                   {
                       { "TokenId", request.Wrapped.TokenId }
                   }))
            {
                _logger.LogError(ex, $"Failure persisting wrapped token.");
            }

            return 0;
        }
    }
}
