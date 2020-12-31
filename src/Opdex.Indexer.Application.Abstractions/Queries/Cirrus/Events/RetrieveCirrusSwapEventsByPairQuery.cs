using System.Collections.Generic;
using Opdex.Indexer.Application.Abstractions.Models.Events;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events
{
    public class RetrieveCirrusSwapEventsByPairQuery : RetrieveCirrusEventBaseQuery<IEnumerable<MintEvent>>
    {
        public RetrieveCirrusSwapEventsByPairQuery(ulong from, ulong to, string pair)
            : base (from, to, pair)
        {
        }
    }
}