using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.EntryHandlers.Blocks
{
    public class GetBestBlockQueryHandler : IRequestHandler<GetBestBlockQuery, BlockReceiptDto>
    {
        private readonly IMediator _mediator;

        public GetBestBlockQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<BlockReceiptDto> Handle(GetBestBlockQuery request, CancellationToken cancellationToken)
        {
            var latestSyncedBlock = await _mediator.Send(new RetrieveLatestBlockQuery(findOrThrow: false), cancellationToken);

            if (latestSyncedBlock != null)
            {
                return await _mediator.Send(new RetrieveCirrusBlockByHashQuery(latestSyncedBlock.Hash), CancellationToken.None);
            }
            
            return await _mediator.Send(new RetrieveCirrusCurrentBlockQuery(), cancellationToken);
        }
    }
}