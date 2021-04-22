using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Transactions
{
    public class GetTransactionsByPoolWithFilterQuery : IRequest<IEnumerable<TransactionDto>>
    {
        public GetTransactionsByPoolWithFilterQuery(string poolAddress, IEnumerable<int> logTypes)
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