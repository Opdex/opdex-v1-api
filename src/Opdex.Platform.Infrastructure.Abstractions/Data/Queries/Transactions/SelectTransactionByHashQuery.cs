using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions
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