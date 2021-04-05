using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions
{
    public class SelectTransactionsByPoolWithFilterQuery : IRequest<IEnumerable<Transaction>>
    {
        public SelectTransactionsByPoolWithFilterQuery(string poolAddress, IEnumerable<int> eventTypes)
        {
            if (!poolAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(poolAddress));
            }

            PoolAddress = poolAddress;
            EventTypes = eventTypes;
        }
        
        public string PoolAddress { get; }
        public IEnumerable<int> EventTypes { get; }
    }
}