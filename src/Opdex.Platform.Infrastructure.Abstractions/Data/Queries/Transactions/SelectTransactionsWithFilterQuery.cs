using MediatR;
using Opdex.Platform.Domain.Models;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions
{
    public class SelectTransactionsWithFilterQuery : IRequest<List<Transaction>>
    {
        public SelectTransactionsWithFilterQuery(string wallet, IEnumerable<uint> includeEvents, IEnumerable<uint> excludeEvents,
                                                 IEnumerable<string> contracts, string direction, ulong limit, long next, long previous)
        {
            Wallet = wallet;
            IncludeEvents = includeEvents;
            ExcludeEvents = excludeEvents;
            Contracts = contracts;
            Direction = direction;
            Limit = limit;
            Next = next;
            Previous = previous;
        }

        public string Wallet { get; }
        public IEnumerable<uint> IncludeEvents { get; }
        public IEnumerable<uint> ExcludeEvents { get; }
        public IEnumerable<string> Contracts { get; }
        public string Direction { get; }
        public ulong Limit { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
