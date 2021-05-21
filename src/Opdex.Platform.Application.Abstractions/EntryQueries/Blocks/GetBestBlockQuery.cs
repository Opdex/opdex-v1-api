using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Blocks
{
    public class GetBestBlockQuery : IRequest<BlockReceiptDto>
    {
        
    }
}