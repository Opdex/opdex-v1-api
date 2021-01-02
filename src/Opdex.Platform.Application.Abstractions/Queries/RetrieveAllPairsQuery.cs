using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.Queries
{
    public class RetrieveAllPairsQuery : IRequest<IEnumerable<PairDto>>
    {
        
    }
}