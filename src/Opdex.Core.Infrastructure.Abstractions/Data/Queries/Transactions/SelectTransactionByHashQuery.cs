using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions
{
    public class SelectTransactionByHashQuery : IRequest<Transaction>
    {
        public SelectTransactionByHashQuery(string hash)
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