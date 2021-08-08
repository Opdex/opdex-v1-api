using MediatR;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Blocks
{
    public class GetBestBlockQuery : IRequest<BlockReceipt>
    {

    }
}