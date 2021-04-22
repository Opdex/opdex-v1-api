using MediatR;
using Opdex.Platform.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Blocks
{
    public class RetrieveLatestBlockQuery : IRequest<BlockDto>
    {
    }
}