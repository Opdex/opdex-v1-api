using System.Collections.Generic;
using MediatR;
using Opdex.Indexer.Application.Abstractions.Models.Events;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events
{
    public class RetrieveCirrusPairEventsQuery : IRequest<IEnumerable<PairEvent>>
    {
        public RetrieveCirrusPairEventsQuery(ulong fromBlock, ulong? toBlock = null)
        {
            FromBlock = fromBlock;
            ToBlock = toBlock;
        }
        
        public ulong FromBlock { get; }
        public ulong? ToBlock { get; }
    }
}