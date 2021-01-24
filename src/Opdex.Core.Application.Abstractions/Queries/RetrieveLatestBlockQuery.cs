using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Application.Abstractions.Queries
{
    public class RetrieveLatestBlockQuery : IRequest<Block>
    {
    }
}