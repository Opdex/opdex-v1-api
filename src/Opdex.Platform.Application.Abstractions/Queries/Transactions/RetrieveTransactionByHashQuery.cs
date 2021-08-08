using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveTransactionByHashQuery : FindQuery<Transaction>
    {
        public RetrieveTransactionByHashQuery(string hash, bool findOrThrow = true) : base(findOrThrow)
        {
            if (!hash.HasValue())
            {
                throw new ArgumentNullException(nameof(hash), "Hash must be provided.");
            }

            Hash = hash;
        }

        public string Hash { get; }
    }
}
