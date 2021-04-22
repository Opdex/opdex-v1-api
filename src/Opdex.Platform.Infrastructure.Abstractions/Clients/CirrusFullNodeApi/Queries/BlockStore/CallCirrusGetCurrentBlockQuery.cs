using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore
{
    public class CallCirrusGetCurrentBlockQuery : IRequest<BlockReceiptDto>
    {
        public CallCirrusGetCurrentBlockQuery()
        {
        }
    }
}