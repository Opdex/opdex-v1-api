using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Indexer.Application.Abstractions.Queries.Blocks
{
    public class RetrieveCirrusCurrentBlockQuery : IRequest<BlockReceiptDto>
    {
        
    }
}