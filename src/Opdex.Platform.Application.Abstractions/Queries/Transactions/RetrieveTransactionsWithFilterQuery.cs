using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveTransactionsWithFilterQuery : IRequest<List<Transaction>>
    {
        public RetrieveTransactionsWithFilterQuery(string wallet, IEnumerable<TransactionEventType> eventTypes, IEnumerable<string> contracts,
                                                   SortDirectionType direction, uint limit, long next, long previous)
        {
            Wallet = wallet;
            Next = next;
            Previous = previous;
            EventTypes = eventTypes ?? Enumerable.Empty<TransactionEventType>();
            Contracts = contracts ?? Enumerable.Empty<string>();
            Direction = direction.IsValid() ? direction : throw new ArgumentOutOfRangeException(nameof(direction), "Invalid sort direction");
            Limit = limit > 0 ? limit : throw new ArgumentOutOfRangeException(nameof(limit), "Invalid limit");
        }

        public string Wallet { get; }
        public IEnumerable<TransactionEventType> EventTypes { get; }
        public IEnumerable<string> Contracts { get; }
        public SortDirectionType Direction { get; }
        public uint Limit { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
