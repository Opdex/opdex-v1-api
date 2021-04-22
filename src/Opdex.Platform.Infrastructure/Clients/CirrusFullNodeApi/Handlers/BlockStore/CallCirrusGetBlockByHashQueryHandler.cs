using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore
{
    public class CallCirrusGetBlockByHashQueryHandler : IRequestHandler<CallCirrusGetBlockByHashQuery, BlockReceiptDto>
    {
        private readonly IBlockStoreModule _blockStoreModule;
        private readonly ILogger<CallCirrusGetBlockByHashQueryHandler> _logger;
        
        public CallCirrusGetBlockByHashQueryHandler(IBlockStoreModule blockStoreModule, 
            ILogger<CallCirrusGetBlockByHashQueryHandler> logger)
        {
            _blockStoreModule = blockStoreModule ?? throw new ArgumentNullException(nameof(blockStoreModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<BlockReceiptDto> Handle(CallCirrusGetBlockByHashQuery request, CancellationToken cancellationToken)
        {
            var result = await _blockStoreModule.GetBlockAsync(request.Hash, cancellationToken);

            return result;
        }
    }
}