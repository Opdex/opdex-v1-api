using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Core.Application.Abstractions.Queries.Blocks
{
    public class RetrieveLatestBlockQuery : IRequest<BlockDto>
    {
    }
}