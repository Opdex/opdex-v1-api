using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Attributes;

public class PersistTokenAttributeCommandHandler : IRequestHandler<PersistTokenAttributeCommand, bool>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO token_attribute (
                {nameof(TokenAttributeEntity.TokenId)},
                {nameof(TokenAttributeEntity.AttributeTypeId)}
              ) VALUES (
                @{nameof(TokenAttributeEntity.TokenId)},
                @{nameof(TokenAttributeEntity.AttributeTypeId)}
              );";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistTokenAttributeCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistTokenAttributeCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(PersistTokenAttributeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<TokenAttributeEntity>(request.Attribute);

            var command = DatabaseQuery.Create(InsertSqlCommand, entity, cancellationToken);

            var result = await _context.ExecuteCommandAsync(command);

            return result > 0;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "TokenId", request.Attribute.TokenId },
                { "AttributeType", request.Attribute.AttributeType }
            }))
            {
                _logger.LogError(ex, $"Failure persisting token attribute.");
            }

            return false;
        }
    }
}