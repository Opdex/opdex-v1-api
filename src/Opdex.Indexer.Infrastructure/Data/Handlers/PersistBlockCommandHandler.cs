using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
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
                {nameof(BlockEntity.MedianTime)}
              ) VALUES (
                @{nameof(BlockEntity.Height)},
                @{nameof(BlockEntity.Hash)},
                @{nameof(BlockEntity.Time)},
                @{nameof(BlockEntity.MedianTime)}
              );";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public PersistBlockCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(PersistBlockCommand request, CancellationToken cancellationToken)
        {
            var blockEntity = _mapper.Map<BlockEntity>(request.Block);
            
            var command = DatabaseQuery.Create(SqlCommand, blockEntity, cancellationToken);
            
            var result = await _context.ExecuteScalarAsync<long>(command);
            
            return result > 0;
        }
    }
}