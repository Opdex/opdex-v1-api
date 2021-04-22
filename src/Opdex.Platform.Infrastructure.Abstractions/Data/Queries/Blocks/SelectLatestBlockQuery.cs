using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks
{
    public class SelectLatestBlockQuery : IRequest<Block>
    {
    }
}