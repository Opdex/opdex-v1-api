using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore
{
    public class CallCirrusGetBlockHashByHeightQueryHandler : IRequestHandler<CallCirrusGetBlockHashByHeightQuery, Sha256>
    {
        private readonly IBlockStoreModule _blockStore;

        public CallCirrusGetBlockHashByHeightQueryHandler(IBlockStoreModule blockStore)
        {
            _blockStore = blockStore ?? throw new ArgumentNullException(nameof(blockStore));
        }

        public async Task<Sha256> Handle(CallCirrusGetBlockHashByHeightQuery request, CancellationToken cancellationToken)
        {
            return await _blockStore.GetBlockHashAsync(request.Height, cancellationToken);
        }
    }
}
