using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions
{
    public class SelectTransactionsByPoolWithFilterQuery : IRequest<IEnumerable<Transaction>>
    {
        public SelectTransactionsByPoolWithFilterQuery(string poolAddress, IEnumerable<int> logTypes)
        {
            if (!poolAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(poolAddress));
            }

            PoolAddress = poolAddress;
            LogTypes = logTypes;
        }
        
        public string PoolAddress { get; }
        public IEnumerable<int> LogTypes { get; }
    }
}