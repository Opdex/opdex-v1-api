using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore
{
    public class CallCirrusGetCurrentBlockQueryHandler : IRequestHandler<CallCirrusGetCurrentBlockQuery, BlockReceiptDto>
    {
        private readonly IBlockStoreModule _blockStore;
        private readonly ILogger<CallCirrusGetCurrentBlockQueryHandler> _logger;
        
        public CallCirrusGetCurrentBlockQueryHandler(IBlockStoreModule blockStore, 
            ILogger<CallCirrusGetCurrentBlockQueryHandler> logger)
        {
            _blockStore = blockStore ?? throw new ArgumentNullException(nameof(blockStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<BlockReceiptDto> Handle(CallCirrusGetCurrentBlockQuery request, CancellationToken cancellationToken)
        {
            var bestsBlockHash = await _blockStore.GetBestBlockAsync(cancellationToken);

            return await _blockStore.GetBlockAsync(bestsBlockHash, cancellationToken);
        }
    }
}