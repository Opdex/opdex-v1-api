using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions
{
    public class SelectTransactionsWithFilterQuery : IRequest<List<Transaction>>
    {
        public SelectTransactionsWithFilterQuery(string wallet, IEnumerable<uint> logTypes, IEnumerable<string> contracts,
                                                 SortDirectionType direction, ulong limit, long next, long previous)
        {
            Wallet = wallet;
            LogTypes = logTypes;
            Contracts = contracts;
            Next = next;
            Previous = previous;
            Direction = direction.IsValid() ? direction : throw new ArgumentOutOfRangeException(nameof(direction), "Invalid sort direction");;
            Limit = limit > 0 ? limit : throw new ArgumentOutOfRangeException(nameof(direction), "Invalid limit");
        }

        public string Wallet { get; }
        public IEnumerable<uint> LogTypes { get; }
        public IEnumerable<string> Contracts { get; }
        public SortDirectionType Direction { get; }
        public ulong Limit { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
