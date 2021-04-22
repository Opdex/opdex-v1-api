using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveTransactionByHashQuery : IRequest<Transaction>
    {
        public RetrieveTransactionByHashQuery(string hash)
        {
            if (!hash.HasValue())
            {
                throw new ArgumentNullException(nameof(hash));
            }

            Hash = hash;
        }
        
        public string Hash { get; }
    }
}