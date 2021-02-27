using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Indexer.Application.Abstractions.Queries.Transactions
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
        
        public string Hash { get; set; }
    }
}