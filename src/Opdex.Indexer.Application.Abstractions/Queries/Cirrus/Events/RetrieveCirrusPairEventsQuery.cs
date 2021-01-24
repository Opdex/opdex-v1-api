using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events
{
    public class RetrieveCirrusPairEventsQuery : IRequest<IEnumerable<PairCreatedEvent>>
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