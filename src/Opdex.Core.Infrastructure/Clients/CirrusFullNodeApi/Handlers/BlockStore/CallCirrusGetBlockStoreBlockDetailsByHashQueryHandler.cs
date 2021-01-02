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
    public class CallCirrusGetBlockStoreBlockDetailsByHashQueryHandler : IRequestHandler<CallCirrusGetBlockStoreBlockDetailsByHashQuery, BlockDto>
    {
        private readonly IBlockStoreModule _blockStoreModule;
        private readonly ILogger<CallCirrusGetBlockStoreBlockDetailsByHashQueryHandler> _logger;
        
        public CallCirrusGetBlockStoreBlockDetailsByHashQueryHandler(IBlockStoreModule blockStoreModule, 
            ILogger<CallCirrusGetBlockStoreBlockDetailsByHashQueryHandler> logger)
        {
            _blockStoreModule = blockStoreModule ?? throw new ArgumentNullException(nameof(blockStoreModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: map the result dto to a domain model
        public async Task<BlockDto> Handle(CallCirrusGetBlockStoreBlockDetailsByHashQuery request, CancellationToken cancellationToken)
        {
            var result = await _blockStoreModule.GetBlockAsync(request.Hash, cancellationToken);

            return result;
        }
    }
}