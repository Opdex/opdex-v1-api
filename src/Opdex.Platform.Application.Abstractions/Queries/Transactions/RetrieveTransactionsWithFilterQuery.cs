using MediatR;
using Opdex.Platform.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveTransactionsWithFilterQuery : IRequest<List<Transaction>>
    {
        public RetrieveTransactionsWithFilterQuery(string wallet, IEnumerable<uint> includeEvents, IEnumerable<uint> excludeEvents,
                                                   IEnumerable<string> contracts, string direction, uint limit, long next, long previous)
        {
            Wallet = wallet;
            IncludeEvents = includeEvents ?? Enumerable.Empty<uint>();
            ExcludeEvents = excludeEvents ?? Enumerable.Empty<uint>();
            Contracts = contracts ?? Enumerable.Empty<string>();
            Direction = direction;
            Limit = limit == 0 ? 10 : limit;
            Next = next;
            Previous = previous;
        }

        public string Wallet { get; }
        public IEnumerable<uint> IncludeEvents { get; }
        public IEnumerable<uint> ExcludeEvents { get; }
        public IEnumerable<string> Contracts { get; }
        public string Direction { get; }
        public uint Limit { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
