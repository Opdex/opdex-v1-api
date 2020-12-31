using System.Collections.Generic;
using Opdex.Indexer.Application.Abstractions.Models.Events;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events
{
    public class RetrieveCirrusPairEventsQuery : RetrieveCirrusEventBaseQuery<IEnumerable<MintEvent>>
    {
        public RetrieveCirrusPairEventsQuery(ulong from, ulong to, string contract)
            : base (from, to, contract)
        {
        }
    }
}