using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Auth;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Auth;

public class PersistAuthSuccessCommandHandler : IRequestHandler<PersistAuthSuccessCommand, bool>
{
    private static readonly string SqlCommand =
        $@"INSERT INTO auth_success (
                {nameof(AuthSuccessEntity.ConnectionId)},
                {nameof(AuthSuccessEntity.Signer)},
                {nameof(AuthSuccessEntity.Expiry)}
              ) VALUES (
                @{nameof(AuthSuccessEntity.ConnectionId)},
                @{nameof(AuthSuccessEntity.Signer)},
                @{nameof(AuthSuccessEntity.Expiry)}
              );".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<PersistAuthSuccessCommandHandler> _logger;

    public PersistAuthSuccessCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistAuthSuccessCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(PersistAuthSuccessCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var authSuccessEntity = _mapper.Map<AuthSuccessEntity>(request.AuthSuccess);

            var command = DatabaseQuery.Create(SqlCommand, authSuccessEntity, cancellationToken);

            var result = await _context.ExecuteCommandAsync(command);

            return result > 0;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
                   {
                       { "ConnectionId", request.AuthSuccess.ConnectionId },
                       { "Signer", request.AuthSuccess.Signer }
                   }))
            {
                _logger.LogError(ex, $"Failure persisting auth success.");
            }
            return false;
        }
    }
}
