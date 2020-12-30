using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;

namespace Opdex.Indexer.Application.Handlers.Cirrus
{
    public class RetrieveCirrusCurrentBlockQueryHandler: IRequestHandler<RetrieveCirrusCurrentBlockQuery, BlockDto>
    {
        public Task<BlockDto> Handle(RetrieveCirrusCurrentBlockQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}