using System.Collections.Generic;
using MediatR;
using Opdex.Indexer.Application.Abstractions.Models.Events;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus
{
    public class RetrieveCirrusMintEventsByPairQuery : IRequest<IEnumerable<MintEvent>>
    {
        public RetrieveCirrusMintEventsByPairQuery(ulong from, ulong to)
        {
            From = from;
            To = to;
        }
        
        public ulong From { get; }
        public ulong To { get; }
    }
}