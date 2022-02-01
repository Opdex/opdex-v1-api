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

public class PersistTokenChainCommandHandler : IRequestHandler<PersistTokenChainCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO token_chain (
                {nameof(TokenChainEntity.TokenId)},
                {nameof(TokenChainEntity.NativeChainTypeId)},
                {nameof(TokenChainEntity.NativeAddress)}
              ) VALUES (
                @{nameof(TokenChainEntity.TokenId)},
                @{nameof(TokenChainEntity.NativeChainTypeId)},
                @{nameof(TokenChainEntity.NativeAddress)}
              );
            SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistTokenChainCommandHandler(IDbContext context, IMapper mapper,
        ILogger<PersistTokenChainCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistTokenChainCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<TokenChainEntity>(request.Chain);

            var command = DatabaseQuery.Create(InsertSqlCommand, entity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return result;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
                   {
                       { "TokenId", request.Chain.TokenId }
                   }))
            {
                _logger.LogError(ex, $"Failure persisting token chain.");
            }

            return 0;
        }
    }
}
