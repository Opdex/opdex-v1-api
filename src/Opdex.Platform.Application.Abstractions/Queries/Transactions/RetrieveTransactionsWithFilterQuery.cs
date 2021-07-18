using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveTransactionsWithFilterQuery : IRequest<List<Transaction>>
    {
        public RetrieveTransactionsWithFilterQuery(string wallet, IEnumerable<TransactionEventType> eventTypes, IEnumerable<string> contracts,
                                                   string direction, uint limit, long next, long previous)
        {
            Wallet = wallet;

            // Todo: need to switch TransactionEventType to TransactionLogType here
            EventTypes = eventTypes ?? Enumerable.Empty<TransactionEventType>();

            Contracts = contracts ?? Enumerable.Empty<string>();
            Direction = direction;
            Limit = limit == 0 ? 10 : limit;
            Next = next;
            Previous = previous;
        }

        public string Wallet { get; }
        public IEnumerable<TransactionEventType> EventTypes { get; }
        public IEnumerable<string> Contracts { get; }
        public string Direction { get; }
        public uint Limit { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
