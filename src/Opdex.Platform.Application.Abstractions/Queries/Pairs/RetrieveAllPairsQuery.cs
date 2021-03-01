using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Pairs
{
    public class RetrieveAllPairsQuery : IRequest<IEnumerable<Pair>>
    {
    }
}