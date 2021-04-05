using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Transactions
{
    public class GetTransactionsByPoolWithFilterQuery : IRequest<IEnumerable<TransactionDto>>
    {
        public GetTransactionsByPoolWithFilterQuery(string poolAddress, IEnumerable<int> eventTypes)
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