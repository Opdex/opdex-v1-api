using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries.Blocks
{
    public class SelectLatestBlockQuery : IRequest<Block>
    {
    }
}