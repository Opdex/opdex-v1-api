using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveCirrusCurrentBlockQuery : IRequest<BlockReceiptDto>
    {
        
    }
}