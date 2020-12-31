using System.Collections.Generic;
using Opdex.Indexer.Application.Abstractions.Models.Events;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events
{
    public class RetrieveCirrusMintEventsByPairQuery : RetrieveCirrusEventBaseQuery<IEnumerable<MintEvent>>
    {
        public RetrieveCirrusMintEventsByPairQuery(ulong from, ulong to, string pair)
            : base (from, to, pair)
        {
        }
    }
}