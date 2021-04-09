using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveTransactionsByPoolWithFilterQuery : IRequest<IEnumerable<Transaction>>
    {
        public RetrieveTransactionsByPoolWithFilterQuery(string poolAddress, IEnumerable<int> logTypes)
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