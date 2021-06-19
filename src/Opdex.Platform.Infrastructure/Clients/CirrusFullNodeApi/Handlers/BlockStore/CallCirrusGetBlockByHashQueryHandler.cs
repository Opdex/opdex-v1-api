using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore
{
    public class CallCirrusGetBlockByHashQueryHandler : IRequestHandler<CallCirrusGetBlockByHashQuery, BlockReceipt>
    {
        private readonly IBlockStoreModule _blockStoreModule;
        private readonly ILogger<CallCirrusGetBlockByHashQueryHandler> _logger;

        public CallCirrusGetBlockByHashQueryHandler(IBlockStoreModule blockStoreModule,
            ILogger<CallCirrusGetBlockByHashQueryHandler> logger)
        {
            _blockStoreModule = blockStoreModule ?? throw new ArgumentNullException(nameof(blockStoreModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BlockReceipt> Handle(CallCirrusGetBlockByHashQuery request, CancellationToken cancellationToken)
        {
            var block = await _blockStoreModule.GetBlockAsync(request.Hash, cancellationToken);

            return new BlockReceipt(block.Hash, block.Height, block.Time.FromUnixTimeSeconds(), block.MedianTime.FromUnixTimeSeconds(),
                block.PreviousBlockHash, block.NextBlockHash, block.MerkleRoot, block.Tx);
        }
    }
}