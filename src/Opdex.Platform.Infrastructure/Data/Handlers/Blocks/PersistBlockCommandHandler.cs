using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using System.Collections.Generic;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Blocks;

public class PersistBlockCommandHandler : IRequestHandler<PersistBlockCommand, bool>
{
    private static readonly string SqlCommand =
        $@"INSERT INTO block (
                {nameof(BlockEntity.Height)},
                {nameof(BlockEntity.Hash)},
                {nameof(BlockEntity.Time)},
                {nameof(BlockEntity.MedianTime)}
              ) VALUES (
                @{nameof(BlockEntity.Height)},
                @{nameof(BlockEntity.Hash)},
                @{nameof(BlockEntity.Time)},
                @{nameof(BlockEntity.MedianTime)}
              );".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistBlockCommandHandler(IDbContext context, IMapper mapper,
                                      ILogger<PersistBlockCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(PersistBlockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var blockEntity = _mapper.Map<BlockEntity>(request.Block);

            var command = DatabaseQuery.Create(SqlCommand, blockEntity, cancellationToken);

            var result = await _context.ExecuteCommandAsync(command);

            return result > 0;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "Hash", request.Block.Hash },
                { "BlockHeight", request.Block.Height }
            }))
            {
                _logger.LogError(ex, $"Failure persisting block.");
            }
            return false;
        }
    }
}