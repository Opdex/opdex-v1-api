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
    public class PersistBlockCommandHandler : IRequestHandler<PersistBlockCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO block (
                {nameof(BlockEntity.Height)},
                {nameof(BlockEntity.Hash)},
                {nameof(BlockEntity.Time)},
                {nameof(BlockEntity.MedianTime)},
                {nameof(BlockEntity.CreatedDate)}
              ) VALUES (
                @{nameof(BlockEntity.Height)},
                @{nameof(BlockEntity.Hash)},
                @{nameof(BlockEntity.Time)},
                @{nameof(BlockEntity.MedianTime)},
                UTC_TIMESTAMP()
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
                _logger.LogError(ex, $"Unable to persist {request.Block}");
                return false;
            }
        }
    }
}