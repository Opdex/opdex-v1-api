using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveAllPoolsQuery : IRequest<IEnumerable<Pool>>
    {
    }
}