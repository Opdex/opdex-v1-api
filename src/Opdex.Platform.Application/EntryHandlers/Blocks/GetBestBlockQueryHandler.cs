using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.EntryHandlers.Blocks
{
    public class GetBestBlockQueryHandler : IRequestHandler<GetBestBlockQuery, BlockReceipt>
    {
        private readonly IMediator _mediator;

        public GetBestBlockQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<BlockReceipt> Handle(GetBestBlockQuery request, CancellationToken cancellationToken)
        {
            // If we don't have a latest block in the database, fetch the chain tip from cirrus. Should only ever be hit during the initial sync
            // Todo: Maybe this should be wiped and we start syncing from the time we deploy our contracts.
            var latestSyncedBlock = await _mediator.Send(new RetrieveLatestBlockQuery(findOrThrow: false), cancellationToken);
            if (latestSyncedBlock == null)
            {
                return await _mediator.Send(new RetrieveCirrusCurrentBlockQuery(), cancellationToken);
            }

            return await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestSyncedBlock.Hash, findOrThrow: false), cancellationToken);
        }
    }
}
