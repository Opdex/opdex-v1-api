using System.Collections.Generic;
using Opdex.Indexer.Application.Abstractions.Models.Events;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events
{
    public class RetrieveCirrusPairEventsQuery : RetrieveCirrusEventBaseQuery<IEnumerable<PairEvent>>
    {
        public RetrieveCirrusPairEventsQuery(string contract, ulong from, ulong? to = null)
            : base (contract, from, to)
        {
        }
    }
}