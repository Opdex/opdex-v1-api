using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pairs
{
    public class GetAllPairsQuery : IRequest<IEnumerable<PairDto>>
    {
        
    }
}