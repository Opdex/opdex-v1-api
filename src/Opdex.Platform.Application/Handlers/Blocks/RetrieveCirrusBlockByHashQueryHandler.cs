using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Handlers.Blocks
{
    public class RetrieveCirrusBlockByHashQueryHandler : IRequestHandler<RetrieveCirrusBlockByHashQuery, BlockReceipt>
    {
        private readonly IMediator _mediator;

        public RetrieveCirrusBlockByHashQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<BlockReceipt> Handle(RetrieveCirrusBlockByHashQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusGetBlockReceiptByHashQuery(request.Hash, request.FindOrThrow), cancellationToken);
        }
    }
}
