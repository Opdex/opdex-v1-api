using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class SelectAddressBalancesWithFilterQuery : IRequest<List<AddressBalance>>
    {
        public SelectAddressBalancesWithFilterQuery(string wallet, IEnumerable<string> tokens, bool includeLpTokens, bool includeZeroBalances,
                                                    SortDirectionType direction, ulong limit, long next, long previous)
        {
            Wallet = wallet;
            Tokens = tokens;
            IncludeLpTokens = includeLpTokens;
            IncludeZeroBalances = includeZeroBalances;
            Next = next;
            Previous = previous;
            Direction = direction.IsValid() ? direction : throw new ArgumentOutOfRangeException(nameof(direction), "Invalid sort direction");;
            Limit = limit > 0 ? limit : throw new ArgumentOutOfRangeException(nameof(direction), "Invalid limit");
        }

        public string Wallet { get; }
        public IEnumerable<string> Tokens { get; }
        public bool IncludeLpTokens { get; }
        public bool IncludeZeroBalances { get; }
        public SortDirectionType Direction { get; }
        public ulong Limit { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
