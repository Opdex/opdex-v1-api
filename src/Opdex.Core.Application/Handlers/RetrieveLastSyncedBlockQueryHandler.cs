using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries;

namespace Opdex.Core.Application.Handlers
{
    public class RetrieveLastSyncedBlockQueryHandler : IRequestHandler<RetrieveLastSyncedBlockQuery, BlockDto>
    {
        public Task<BlockDto> Handle(RetrieveLastSyncedBlockQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}