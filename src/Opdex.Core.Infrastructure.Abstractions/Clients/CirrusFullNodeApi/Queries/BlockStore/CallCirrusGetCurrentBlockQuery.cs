using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore
{
    public class CallCirrusGetCurrentBlockQuery : IRequest<BlockReceiptDto>
    {
        public CallCirrusGetCurrentBlockQuery()
        {
        }
    }
}