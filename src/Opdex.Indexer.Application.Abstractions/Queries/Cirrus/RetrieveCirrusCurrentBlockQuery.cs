using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus
{
    public class RetrieveCirrusCurrentBlockQuery : IRequest<BlockReceiptDto>
    {
        
    }
}