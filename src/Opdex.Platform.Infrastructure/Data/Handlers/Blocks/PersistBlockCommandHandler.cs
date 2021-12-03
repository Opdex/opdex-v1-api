using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;

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
              );";

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
            _logger.LogError(ex, $"Failure persisting {request.Block}.");
                
            return false;
        }
    }
}