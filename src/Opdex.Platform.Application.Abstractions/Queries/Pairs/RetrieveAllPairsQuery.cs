using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Pairs
{
    public class RetrieveAllPairsQuery : IRequest<IEnumerable<PairDto>>
    {
        
    }
}