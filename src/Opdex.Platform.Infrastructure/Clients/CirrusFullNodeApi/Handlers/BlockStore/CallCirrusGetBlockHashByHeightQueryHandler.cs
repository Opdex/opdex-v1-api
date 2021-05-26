using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore
{
    public class CallCirrusGetBlockHashByHeightQueryHandler : IRequestHandler<CallCirrusGetBlockHashByHeightQuery, string>
    {
        private readonly IBlockStoreModule _blockStore;
        private readonly ILogger<CallCirrusGetBlockHashByHeightQueryHandler> _logger;
        
        public CallCirrusGetBlockHashByHeightQueryHandler(IBlockStoreModule blockStore, 
            ILogger<CallCirrusGetBlockHashByHeightQueryHandler> logger)
        {
            _blockStore = blockStore ?? throw new ArgumentNullException(nameof(blockStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<string> Handle(CallCirrusGetBlockHashByHeightQuery request, CancellationToken cancellationToken)
        {
            return await _blockStore.GetBlockHashAsync(request.Height, cancellationToken);
        }
    }
}