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
    public class CallCirrusGetCurrentBlockQueryHandler : IRequestHandler<CallCirrusGetCurrentBlockQuery, BlockReceipt>
    {
        private readonly IBlockStoreModule _blockStore;
        private readonly ILogger<CallCirrusGetCurrentBlockQueryHandler> _logger;

        public CallCirrusGetCurrentBlockQueryHandler(IBlockStoreModule blockStore,
            ILogger<CallCirrusGetCurrentBlockQueryHandler> logger)
        {
            _blockStore = blockStore ?? throw new ArgumentNullException(nameof(blockStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BlockReceipt> Handle(CallCirrusGetCurrentBlockQuery request, CancellationToken cancellationToken)
        {
            var bestsBlockHash = await _blockStore.GetBestBlockAsync(cancellationToken);
            var block = await _blockStore.GetBlockAsync(bestsBlockHash, cancellationToken);

            return new BlockReceipt(block.Hash, block.Height, block.Time.FromUnixTimeSeconds(), block.MedianTime.FromUnixTimeSeconds(),
                block.PreviousBlockHash, block.NextBlockHash, block.MerkleRoot, block.Tx);
        }
    }
}
