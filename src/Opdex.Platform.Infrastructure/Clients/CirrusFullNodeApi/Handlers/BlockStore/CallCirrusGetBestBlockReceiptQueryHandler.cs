using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;

public class CallCirrusGetBestBlockReceiptQueryHandler : IRequestHandler<CallCirrusGetBestBlockReceiptQuery, BlockReceipt>
{
    private readonly IBlockStoreModule _blockStore;

    public CallCirrusGetBestBlockReceiptQueryHandler(IBlockStoreModule blockStore)
    {
        _blockStore = blockStore ?? throw new ArgumentNullException(nameof(blockStore));
    }

    public async Task<BlockReceipt> Handle(CallCirrusGetBestBlockReceiptQuery request, CancellationToken cancellationToken)
    {
        var bestsBlockHash = await _blockStore.GetBestBlockAsync(cancellationToken);
        var block = await _blockStore.GetBlockAsync(bestsBlockHash, cancellationToken);

        return new BlockReceipt(block.Hash, block.Height, block.Time.FromUnixTimeSeconds(), block.MedianTime.FromUnixTimeSeconds(),
                                block.PreviousBlockHash, block.NextBlockHash, block.MerkleRoot, block.SmartContractCallTxs);
    }
}
